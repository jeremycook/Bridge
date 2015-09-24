using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bridge.Tests.Models;
using System.Linq;

namespace Bridge.EF.Tests
{
    [TestClass]
    public class ReadTests : TestsBase
    {
        [TestMethod]
        public void GetPost()
        {
            var post = PostHydrador.GetSingle();
            Bridge.Insert(post);

            var receivedPost = Bridge.Get<Post>(post.Id);

            Assert.IsNotNull(receivedPost);
            Assert.AreEqual(post.Id, receivedPost.Id);
        }

        [TestMethod]
        public void GetPosts()
        {
            var range = Bridge.Query<Post>();
            int startCount = range.ToList().Count;

            var posts = PostHydrador.GetList(1000);
            Bridge.InsertRange(posts);

            var receivedPosts = range.ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(posts.Count, receivedPosts.Count - startCount);
        }

        [TestMethod]
        public void GetFilteredPosts()
        {
            var range = Bridge.Query<Post>();
            int startCount = range.ToList().Count;

            var posts = PostHydrador.GetList(1000);
            Bridge.InsertRange(posts);

            string contentFullName = typeof(IContent).FullName;
            var receivedPosts = range
                .Filter(new And(
                    leftFilter: new Eq(new Field(nameof(Post.Title)), new Literal(posts.First().Title)),
                    rightFilter: new Eq(new Field(nameof(Post.Title)), new Literal(posts.First().Title))
                ))
                .ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(0, receivedPosts.Count);
        }

        [TestMethod]
        public void GetSortedPosts()
        {
            var range = Bridge.Query<Post>();
            int startCount = range.ToList().Count;

            var posts = PostHydrador.GetList(1000);
            Bridge.InsertRange(posts);

            var receivedPosts = range
                .Sort(new IndexSort("Name"))
                .ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(posts.Count, receivedPosts.Count - startCount);
        }

        [TestMethod]
        public void GetPagedPosts()
        {
            var posts = PostHydrador.GetList(50);
            Bridge.InsertRange(posts);

            var range = Bridge.Query<Post>();
            var receivedPosts = range
                .Page(20)
                .ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(20, receivedPosts.Count);
        }
    }
}
