using AqLife.Application.Business.Account.Services;
using AqLife.CoreTest.IntegrationTest;
using AqLife.CoreTest.TestData;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Extensions.Database;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Utils;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Testing.Platform.Services;
using Npgsql;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


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
        public async Task CreateAccount_ShouldCreateAccount()
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", key);

            var loginName = await ExecuteAsync(command,CancellationToken.None);

            var account = await VerifyAsync(loginName);

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
        public async Task CreateAccount_NormalAccount(string testSource)
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");
            // 符合规则的测试数据
            CreateAccountCommand command = new(testSource, "测试账户描述", "pwd123456", key);

            // 执行测试
            var loginName = await ExecuteAsync(command,CancellationToken.None);
            // 检查测试结果
            var account = await VerifyAsync(loginName);

            var pwdHash = FastHash.GetSha256Hash(command.Pwd);
            Assert.NotNull(account); // 这里通过即为PASS
            Assert.Equal(testSource, account.Name);
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
        public async Task CreateAccount_EmptyNameAccount(string testSource)
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");

            // 符合规则的测试数据
            CreateAccountCommand command = new(testSource, "测试账户描述", "pwd123456", key);

            // 执行测试
            var exception=await Assert.ThrowsAsync<RequestCheckException>(async() =>await ExecuteAsync(command,CancellationToken.None) );
            // 检查测试结果
            Assert.Equal("账户名称不允许存在空格", exception.Message);
            var account = await SearchAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS                                  
        }
        /// <summary>
        /// 验证需求三:超长用户名
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task CreateAccount_LongNameAccount()
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");
            // 符合规则的测试数据
            string longName = new string('a',33);
            CreateAccountCommand command = new(longName, "测试账户描述", "pwd123456", key);

            // 执行测试
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async ()=>await ExecuteAsync(command,CancellationToken.None));
            // 检查测试结果
            var postgresException = Assert.IsType<PostgresException>(exception.InnerException);

            Assert.Equal("22001", postgresException.SqlState);
            Assert.Contains("value too long for type character varying(32)",postgresException.Message);
            var account = await SearchAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS        
        }
        /// <summary>
        /// 验证需求六 : 拦截超长Desc
        /// </summary>
        /// <returns></returns>
        [Theory, ResetDatabase]
        [MemberData(nameof(AccountTestData.InvalidDescData), MemberType = typeof(AccountTestData))]
        public async Task CreateAccount_LongDescAccount(string testSource)
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", testSource, "pwd123456", key);

            // 执行测试
            // 检查测试结果
            var exception = await Assert.ThrowsAsync<DbUpdateException>(async () =>await ExecuteAsync(command,CancellationToken.None));
            var account = await SearchAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS        
        }
        /// <summary>
        /// 验证需: 上传有效的Command.Desc 成功
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Theory,ResetDatabase]
        [MemberData(nameof(AccountTestData.ValidDescData), MemberType = typeof(AccountTestData))]
        public async Task CreateAccount_ShouldValidDesc(string desc)
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", desc, "pwd123456", key);
            var loginName = await ExecuteAsync(command,CancellationToken.None);
            var account = await VerifyAsync(loginName);
            Assert.NotNull(account);
            Assert.Equal(desc,account.Desc);
        }


        /// <summary>
        /// 验证需求四: 拦截重复注册 | 拦截重复用户名
        /// </summary>
        /// <returns></returns>
        [Fact, ResetDatabase]
        public async Task CreateAccount_DuplicateAccountName()
        {
            string key = await base.EnsureSystemKeyAsync() ?? throw new Exception("意外的错误:来自SystemInitializationService");
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", key);

            // 执行测试
            var loginName = await ExecuteAsync(command,CancellationToken.None);
            // 确保第一次注册成功
            var account = await VerifyAsync(loginName);
            Assert.NotNull(account);
            Assert.True(account.IsValid);

            var exception =await Assert.ThrowsAsync<RequestCheckException>(async () => await ExecuteAsync(command,CancellationToken.None));
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
        public async Task CreateAccount_ErrorServerKey(string testSource)
        {
            // 符合规则的测试数据            
            CreateAccountCommand command = new("测试账户1", "测试账户描述", "pwd123456", testSource);

            // 执行测试
            var exception = await Assert.ThrowsAsync<RequestCheckException>(async () =>await ExecuteAsync(command,CancellationToken.None));
            // 检查测试结果
            Assert.Equal("系统密钥不匹配", exception.Message);
            var account = await SearchAsync(command.Name);
            Assert.Null(account); // 这里通过即为PASS
        }
        
        private async Task<AccountEntity?> VerifyAsync(string loginName)
        => await base.QueryAsync(e => e.LoginName == loginName);

        private async Task<AccountEntity?> SearchAsync(string name)
        => await base.QueryAsync(e => e.Name == name);
    }
}
