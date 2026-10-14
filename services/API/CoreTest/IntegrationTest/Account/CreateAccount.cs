using AqLife.CoreTest.TestData;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace AqLife.CoreTest.IntegrationTest.Account
{
    [Collection<IntegrationTestCollection>]
    public class CreateAccount(IntegrationTestFixture fixture) : AccountTest(fixture)
    {
        /// <summary>
        /// 验证需求: 正常密钥注册成功
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Fact, ResetDatabase]
        public async Task CreateAccount_ValidSystemKey_ShouldCreateAccount()
        {
            string key = await base.EnsureSystemKeyAsync();
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", key);

            var loginName = await ExecuteAsync(command, CancellationToken.None);

            var account = await FindByLoginNameAsync(loginName);

            var pwdHash = FastHash.GetSha256Hash(command.Pwd);

            Assert.NotNull(account);
            Assert.Equal("测试账户1", account.Name);
            Assert.Equal("测试账户描述", account.Desc);
            Assert.Equal(pwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
        }

        /// <summary>
        /// 验证需求一: 正常用户名
        /// </summary>
        /// <returns></returns>
        [InlineData("1")]
        [InlineData("a")]
        [InlineData("一")]
        [InlineData("\\")]
        [Theory, ResetDatabase]
        public async Task CreateAccount_ValidName_ShouldSucceed(string validName)
        {
            string key = await base.EnsureSystemKeyAsync();
            // 符合规则的测试数据
            CreateAccountCommand command = new(validName, "测试账户描述", "pwd123456", key);

            // 执行测试
            var loginName = await ExecuteAsync(command, CancellationToken.None);
            // 检查测试结果
            var account = await FindByLoginNameAsync(loginName);

            var pwdHash = FastHash.GetSha256Hash(command.Pwd);
            Assert.NotNull(account); // 这里通过即为PASS
            Assert.Equal(validName, account.Name);
            Assert.Equal(command.Desc, account.Desc);
            Assert.Equal(pwdHash, account.LoginPasswordHash);
            Assert.True(account.IsValid);
        }

        /// <summary>
        /// 验证需求二: 空用户名
        /// </summary>
        /// <returns></returns>
        /// 
        [InlineData("")]
        [InlineData("ab ")]
        [InlineData("a b c")]
        [Theory, ResetDatabase]
        public async Task CreateAccount_NameWithWhitespace_ShouldReject(string invalidName)
        {
            string key = await base.EnsureSystemKeyAsync();

            // 符合规则的测试数据
            CreateAccountCommand command = new(invalidName, "测试账户描述", "pwd123456", key);

            // 执行测试
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command, CancellationToken.None));
            // 检查测试结果
            Assert.Equal("账户名称不允许存在空格", exception.Message);
            var account = await FindByNameAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS                                  
        }
        /// <summary>
        /// 验证需求三:超长用户名
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task CreateAccount_OverlengthName_ShouldRejectByDatabase()// Name 列长度上限为 32
        {
            string key = await base.EnsureSystemKeyAsync();
            // 符合规则的测试数据
            string longName = new string('a', 33);
            CreateAccountCommand command = new(longName, "测试账户描述", "pwd123456", key);

            // 执行测试
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () => await ExecuteAsync(command, CancellationToken.None));
            // 检查测试结果
            var postgresException = Assert.IsType<PostgresException>(exception.InnerException);

            Assert.Equal("22001", postgresException.SqlState);
            Assert.Contains("value too long for type character varying(32)", postgresException.Message);
            var account = await FindByNameAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS        
        }
        /// <summary>
        /// 验证需求六 : 拦截超长Desc
        /// </summary>
        /// <returns></returns>
        [Theory, ResetDatabase]
        [MemberData(nameof(AccountTestData.InvalidDescData), MemberType = typeof(AccountTestData))]
        public async Task CreateAccount_OverlengthDesc_ShouldRejectByDatabase(string overlengthDesc)
        {
            string key = await base.EnsureSystemKeyAsync();
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", overlengthDesc, "pwd123456", key);

            // 执行测试
            // 检查测试结果
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () => await ExecuteAsync(command, CancellationToken.None));
            var account = await FindByNameAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS        
        }
        /// <summary>
        /// 验证需: 上传有效的Command.Desc 成功
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Theory, ResetDatabase]
        [MemberData(nameof(AccountTestData.ValidDescData), MemberType = typeof(AccountTestData))]
        public async Task CreateAccount_ValidDesc_ShouldSucceed(string validDesc)
        {
            string key = await base.EnsureSystemKeyAsync();
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", validDesc, "pwd123456", key);
            var loginName = await ExecuteAsync(command, CancellationToken.None);
            var account = await FindByLoginNameAsync(loginName);
            Assert.NotNull(account);
            Assert.Equal(validDesc, account.Desc);
        }


        /// <summary>
        /// 验证需求四: 拦截重复注册 | 拦截重复用户名
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task CreateAccount_DuplicateRegistration_ShouldRejectSecond()
        {
            string key = await base.EnsureSystemKeyAsync();
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", key);

            // 执行测试
            var loginName = await ExecuteAsync(command, CancellationToken.None);
            // 确保第一次注册成功
            var firstAccount = await FindByLoginNameAsync(loginName);
            Assert.NotNull(firstAccount);
            Assert.True(firstAccount.IsValid);

            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command, CancellationToken.None));
            Assert.Equal("仅允许注册一个账户", exception.Message);
        }

        /// <summary>
        /// 验证需求五: 拦截错误密钥
        /// </summary>
        /// <param name="testSource"></param>
        /// <returns></returns>
        [InlineData("")]
        [InlineData("9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08")]
        [Theory, ResetDatabase]
        public async Task CreateAccount_InvalidSystemKey_ShouldReject(string invalidServerKey)
        {
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", invalidServerKey);

            // 执行测试
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command, CancellationToken.None));
            // 检查测试结果
            Assert.Equal("系统密钥不匹配", exception.Message);
            var account = await FindByNameAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS
        }

        private async Task<AccountEntity?> FindByLoginNameAsync(string loginName)
        => await base.QueryAsync(e => e.LoginName == loginName);

        private async Task<AccountEntity?> FindByNameAsync(string name)
        => await base.QueryAsync(e => e.Name == name);
    }
}
