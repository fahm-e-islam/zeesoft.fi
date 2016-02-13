using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication2.Helper;
using Newtonsoft.Json;
using ZeeSoft.ClassRoomJson;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           // var msg = JsonConvert.SerializeObject<Message>(new Message { SenderId = Guid.Parse("7a6d1634-7071-4193-90d4-3cda61d30664"), Body = "test", RecipientIds = new System.Collections.Generic.List<Guid> { } });

            var calendar=Utils.GetCalendarDaysFor(1);
            //Assert.AreSame(calendar.)
        }
    }
}
