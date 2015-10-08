# Bridge

Use it today to build a bridge between your SQL database and your NoSQL project needs.
With it you can do all the usual things like create, read, update, and delete.
Plus you can do more interesting things like query by class or interface, filter results, sort, and page.
(Yes I said query by interface: query everything that implements a particular interface!)

```csharp
var bridge = new DbBridge(new SqlConnection(@"Data Source=.\sqlexpress; Initial Catalog=BridgeDb; Integrated Security=True"));

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
```

## License

[MIT](License.txt)
