using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBridge.Tests.Models;
using System.Linq;
using System.Collections.Generic;

namespace DBridge.Tests.Db
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
            int startCount = range.Count();

            var posts = PostHydrador.GetList(1000);
            Bridge.InsertRange(posts);

            var receivedPosts = range.ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(posts.Count, receivedPosts.Count - startCount);
        }

        [TestMethod]
        public void GetTitledModels()
        {
            int startCount = Bridge.Query<Post>().Count();

            var titledModels = PostHydrador.GetList(800).Cast<ITitled>()
                .Union(LinkHydrador.GetList(200))
                .ToList();
            Bridge.InsertRange(titledModels);

            var receivedPosts = Bridge.Query<ITitled>().ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(titledModels.Count, receivedPosts.Count - startCount);
        }

        [TestMethod]
        public void GetFilteredPosts()
        {
            int startCount = Bridge.Query<Post>().Count();

            var posts = PostHydrador.GetList(1000);
            Bridge.InsertRange(posts);

            string contentFullName = typeof(IContent).FullName;
            var receivedPosts = Bridge.Query<Post>()
                .Filter(new And(
                    leftFilter: new Eq(new Field(nameof(Post.Title)), new Literal(posts.First().Title)),
                    rightFilter: new Eq(new Field(nameof(Post.Author)), new Literal(posts.First().Author))
                ))
                .ToList();

            Assert.IsNotNull(receivedPosts);
            Assert.AreEqual(1, receivedPosts.Count);
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
