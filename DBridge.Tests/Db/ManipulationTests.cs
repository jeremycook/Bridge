using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DBridge.Tests.Db
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
