# DataBridge

NoSQL design ♥ SQL database

I've made this project to scratch an itch. I want to use a NoSQL store like Mongo,
Raven or whatever, but don't have access to one. What I do have
access to is SQL Server. Thus was born DataBridge, a persistance layer that allows
me to forget about the **RM** in ORMs, and just get on with the **O**s.

Use it today to build a bridge between your SQL database and your NoSQL project 
needs. With it you can do all the usual things like create, read, update, and delete
 -- plus more interesting things like query by class or interface, filter results, 
sort, and page. (Yes, query everything that implements a particular interface, so handy!)

Feedback, issues, and pull requests are all welcome!

## Basic examples

This is what you'll be writing.

```csharp
using (var connection = new SqlConnection(@"Data Source=.\sqlexpress; Initial Catalog=BridgeDb; Integrated Security=True"))
using (var bridge = new DbBridge(connection))
{
    // Create
    bridge.Insert(post);
    bridge.InsertRange(posts);

    // Read
    var post = bridge.Get<Post>(post.Id);

    // Query by class
    var posts = bridge.Query<Post>().ToList();

    // Query by interface
    var posts = bridge.Query<ITitled>().ToList();

    // Filter, sort, and page
    var receivedPosts = bridge.Query<Post>()
        .Filter(new And(
            leftFilter: new Eq(new Field(nameof(Post.Title)), new Literal(posts.First().Title)),
            rightFilter: new Eq(new Field(nameof(Post.Author)), new Literal(posts.First().Author))
        ))
        .Sort(new IndexSort("Name"))
        .Page(20)
        .ToList();

    // Update
    bridge.Update(post.Id, post);

    // Delete
    bridge.Delete(post.Id);
    bridge.DeleteRange(posts.Select(o => o.Id));
}
```

## SQL schema

SQL needs schema like peanut butter needs jelly.

This is the schema `DbBridge` uses behind the scenes to persist and query your 
objects. This may be formalized into a simple yet automated migration system in the 
future, but for now just run this script to get yours database ready for `DbBridge`.

### SQL Server

```sql
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
SET ANSI_PADDING ON

GO

CREATE TABLE [dbo].[Classes](
	[Name] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_dbo.Classes] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[FieldIndexes](
	[RecordId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](442) NOT NULL,
	[Guid] [uniqueidentifier] NULL,
	[Text] [nvarchar](450) NULL,
	[Moment] [datetimeoffset](7) NULL,
	[Number] [decimal](18, 2) NULL,
	[Float] [real] NULL,
 CONSTRAINT [PK_dbo.FieldIndexes] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Interfaces](
	[ClassName] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_dbo.Interfaces] PRIMARY KEY CLUSTERED 
(
	[ClassName] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Records](
	[Id] [uniqueidentifier] NOT NULL,
	[ClassName] [nvarchar](250) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Storage] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_dbo.Records] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF

GO

ALTER TABLE [dbo].[FieldIndexes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.FieldIndexes_dbo.Records_RecordId] FOREIGN KEY([RecordId])
REFERENCES [dbo].[Records] ([Id])
ON DELETE CASCADE

GO

ALTER TABLE [dbo].[FieldIndexes] CHECK CONSTRAINT [FK_dbo.FieldIndexes_dbo.Records_RecordId]

GO

ALTER TABLE [dbo].[Interfaces]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Interfaces_dbo.Classes_ClassName] FOREIGN KEY([ClassName])
REFERENCES [dbo].[Classes] ([Name])
ON DELETE CASCADE

GO

ALTER TABLE [dbo].[Interfaces] CHECK CONSTRAINT [FK_dbo.Interfaces_dbo.Classes_ClassName]

GO

ALTER TABLE [dbo].[Records]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Records_dbo.Classes_ClassName] FOREIGN KEY([ClassName])
REFERENCES [dbo].[Classes] ([Name])
ON DELETE CASCADE

GO

ALTER TABLE [dbo].[Records] CHECK CONSTRAINT [FK_dbo.Records_dbo.Classes_ClassName]
```

## Plans, desires and footnotes

* Great `IDbConnection` support: Support *any* or at least the most common
  implementations of `IDbConnection`. SQLite seems like a great starting point.
* `DataBridge.Mongo` support: I can imagine starting a project with DataBridge over SQL and
  later wanting to use DataBridge over Mongo in order to benefit from MapReduce and other
  features.

> The projects I am using DataBridge on currently target SQL Server, and that means certain
aspects of DataBridge may only work with a SQL Server database. But, I desire to eventually
support any database that [Dapper](https://github.com/StackExchange/dapper-dot-net) supports
because Dapper is what I'm using to interact with the database under the hood. If 
you find DataBridge useful and need, would like, or have implemented support for another 
database [please let me know](https://github.com/jeremycook/DataBridge/issues).

## License

[MIT](License.txt)
