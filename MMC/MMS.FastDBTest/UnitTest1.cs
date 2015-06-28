using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMC.FastDB;

namespace MMS.FastDBTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //
            // 存储测试
            //
            using (var db = new Database("db.db"))
            {
                var entityTable = db.GetTable<Entity1>();

                for (int i = 0; i < 100000; i++)
                {
                    var entity1 = new Entity1();
                    entity1.A = 0;
                    entity1.B = 2.0f;
                    entity1.C = 3.222;
                    entity1.D = "string" + i;
                    entityTable.Insert(entity1);
                }
            }

            //
            // 数据访问测试
            //

            using (var db = new Database("db.db"))
            {
                var entityData = db.QueryTable<Entity1>();

                if (entityData != null)
                {
                    //
                    // 所有数据遍历
                    //

                    // 直接foreach遍历
                    foreach (var entity1 in entityData)
                    {
                        Console.WriteLine("Entity A = " + entity1.A + ", B = " + entity1.B + ", C = " + entity1.C + ", D = " + entity1.D);
                    }

                    // 使用索引访问
                    for (int i = 0; i < entityData.Count; i++)
                    {
                        var entity1 = entityData[i];
                        Console.WriteLine("Entity A = " + entity1.A + ", B = " + entity1.B + ", C = " + entity1.C + ", D = " + entity1.D);
                    }
                }
            }
        }
    }


    public class Entity1
    {
        public int A;
        public float B;
        public double C;
        public string D;
    }
}
