# ASP 类库

> 对于命名空间，一定不能使用系统标识符，如SQL等系统关键字，开发文件夹一定**不能使用系统关键字**

| 类库名                                | 类库用途                          |
| ------------------------------------- | --------------------------------- |
| `**System.Data.SqlClient;**`          | 用于创建数据库对象                |
| `**System.Configuration;**`           | 用于设置`XML`方式的数据库连接方式 |
| `**System.Text.RegularExpressions;**` | `C#` 正则匹配                     |

## ADO.NET 常用数据操作

| 方法                                                         | 用途                                        |
| ------------------------------------------------------------ | ------------------------------------------- |
| `SqlConnection( {SqlObjName} )`                              | 创建一个以指定用名的数据库对象              |
| `SqlCommand()`                                               | 创建一个数据库指令对象                      |
| `{SqlObjMand}.Parameters.AddWithValue({String,控件值\|数据值})` | 绑定一个`SQL`指令元素值                     |
| `{SqlObjMand}.ExecuteReader()`                               | `SQL`指令{查}执行方法                       |
| `{SqlObjMand}.ExecuteNonQuery()`                             | `SQL`指令{增，删，改}执行方法               |
| `{SqlObjMand}.ExecuteScalar()`                               | `SQL`指令{查[带聚合函数\|单查询]}执行方法   |
| `{SqlObjMand}.ExecuteNonQueryAsync()`                        | `SQL`指令{**链接数据库**并执行查询}执行方法 |
| `{SqlObjMand}.SqlCommandBuilder`                             |                                             |

| 属性                           | 描述                  |
| ------------------------------ | --------------------- |
| `**{SqlObjMand}.Connection**`  | `SQL`指令的数据库环境 |
| `**{SqlObjMand}.CommandText**` | `SQL`指令的操作命令   |
| `**{SqlObjMand}.CommandType**` | `SQL`指令类型         |

## 离线访问

| 方法                                                      | 描述                        |
| --------------------------------------------------------- | --------------------------- |
| `DataSet()`                                               | 创建一个离线访问对象        |
| `SqlDataAdapter(String SQL_Command,SqlConnection SqlObj)` | 创建一个离线访问的`SQL`语句 |
| `{SqlDataAdapterObj}.Fill(DataSet SQl,String TableName)`  | 以`TableName`表填充至本元素 |

### 离线访问涉及数据类型

| 数据类型            | 涉及方法\|对象\|属性 | 描述                                  |
| ------------------- | -------------------- | ------------------------------------- |
| `DataSet`           | `DataSet()`          | 离线访问对象                          |
| `DataTable`         | `Object`             | 离线存储的数据**表**                  |
| `DataRowCollection` | `Object`             | 离线存储的**行集合**                  |
| `DataRow`           | `Object`             | 离线存储的**行数据**                  |
| `SqlDataAdapter`    | `Object`             | 填充`DataSet`对象和数据库的命令和连接 |

## C# 节点记录

## C# 正则

| `方法`                                                       | `描述`                                                       |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| `**public bool IsMatch( string input )**`                    | 指示 `Regex` 构造函数中指定的正则表达式是否在指定的输入字符串中找到匹配项。 |
| `**public bool IsMatch( string input, int startat )**`       | 指示 `Regex` 构造函数中指定的正则表达式是否在指定的输入字符串中找到匹配项，从字符串中指定的开始位置开始。 |
| `**public static bool IsMatch( string input, string pattern )**` | 指示指定的正则表达式是否在指定的输入字符串中找到匹配项。     |
| `**public MatchCollection Matches( string input )**`         | 在指定的输入字符串中搜索正则表达式的所有匹配项。             |
| `**public string Replace( string input, string replacement )**` | 在指定的输入字符串中，把所有匹配正则表达式模式的所有匹配的字符串替换为指定的替换字符串。 |
| `**public string[] Split( string input )**`                  | 把输入字符串分割为子字符串数组，根据在 `Regex` 构造函数中指定的正则表达式模式定义的位置进行分割。 |

## C# 类修饰符

| 修饰名                   | 描述                                         |
| ------------------------ | -------------------------------------------- |
| `**private**`            | 私有，仅限成员的父类内部可以使用             |
| `**protected**`          | 私有继承，仅限成员父类和继承子类内部可以使用 |
| `**internal**`           | 程序集私有，其他程序不可使用                 |
| `**protected internal**` | 访问当前程序集或派生包含的类型               |
| `**public**`             | 全局公有，所有方法均可访问                   |
| `**Static**`             | 静态全局属性，所有对象均可访问               |
