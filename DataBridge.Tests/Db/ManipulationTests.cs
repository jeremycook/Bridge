using DataBridge.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DataBridge.Tests.Db
{
    [TestClass]
    public class ManipulationTests : TestsBase
    {
        [TestMethod]
        public void InsertPost()
        {
            var post = PostHydrador.GetSingle();

            Bridge.Insert(post);
        }

        [TestMethod]
        public void InsertPosts()
        {
            var posts = PostHydrador.GetList(1000);

            Bridge.InsertRange(posts);
        }

        [TestMethod]
        public void UpdatePost()
        {
            var post = PostHydrador.GetSingle();
            Bridge.Insert(post);

            post.Title = "Changed title!";

            Bridge.Update(post.Id, post);

            post = Bridge.Get(post.Id) as Post;
            Assert.AreEqual("Changed title!", post.Title);
        }

        [TestMethod]
        public void UpdatePostUpdatesIndexes()
        {
            var post = PostHydrador.GetSingle();
            Bridge.Insert(post);

            post.Title = "Changed title!";

            Bridge.Update(post.Id, post);

            var match = Bridge.Query<Post>()
                .Filter(new Eq(new Field("Title"), new Literal("Changed title!")))
                .Count();

            Assert.AreEqual(1, match);
        }

        [TestMethod]
        public void DeletePost()
        {
            var post = PostHydrador.GetSingle();
            Bridge.Insert(post);

            Bridge.Delete(post.Id);
        }

        [TestMethod]
        public void DeletePosts()
        {
            var posts = PostHydrador.GetList(1000);
            Bridge.InsertRange(posts);

            Bridge.DeleteRange(posts.Select(o => o.Id));
        }
    }
}
