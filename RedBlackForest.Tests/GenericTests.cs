﻿using System;
using System.Linq;
using NUnit.Framework;

namespace RedBlackForest.Tests
{
    [TestFixture]
    public class GenericTests
    {
        [Test]
        public void AddShouldFailWithArgumentNullExceptionWhenDuplicateItemsProvided()
        {
            Assert.That(() => new RedBlackTree<Int32, String> {{0, "Zero"}, {0, "Zero"}}, Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void AddOrGetShouldAddWhenValueNotExists()
        {
            var tree = new RedBlackTree<Int32, String> { { 0, "Zero" }, { 1, "One" } };

            var result1 = tree.TryGetOrAddValue(2, "Two");
            var result2 = tree[2];

            Assert.That(result1, Is.EqualTo("Two"));
            Assert.That(result2, Is.EqualTo("Two"));

            var result3 = tree.TryGetOrAddValue(3, () => "Three");
            var result4 = tree.TryGetOrAddValue(3, () => "Four");
            var result5 = tree[3];

            Assert.That(result3, Is.EqualTo("Three"));
            Assert.That(result4, Is.EqualTo("Three"));
            Assert.That(result5, Is.EqualTo("Three"));
        }


        [Test]
        public void AddOrGetShouldGetWhenValueAlreadyExists()
        {
            var tree = new RedBlackTree<Int32, String> { { 0, "Zero" }, { 1, "One" } };

            var result1 = tree.TryGetOrAddValue(0, "Two");

            var result2 = tree[0];

            Assert.That(result1, Is.EqualTo("Zero"));
            Assert.That(result2, Is.EqualTo("Zero"));
        }

        [Test]
        public void NearestNodesShouldReturnProperValues()
        {
            var tree = new RedBlackTree<Int32, String>();

            var resultZ = tree.NearestPairs(5);

            tree.Add(2, "Two");
            tree.Add(5, "Five");
            tree.Add(8, "Eight");

            var result0 = tree.NearestPairs(0);
            var result2 = tree.NearestPairs(2);
            var result3 = tree.NearestPairs(3);
            var result5 = tree.NearestPairs(5);
            var result6 = tree.NearestPairs(6);
            var result8 = tree.NearestPairs(8);
            var result9 = tree.NearestPairs(9);

            Assert.That(resultZ.IsSetA, Is.False);
            Assert.That(resultZ.IsSetB, Is.False);

            Assert.That(result0.IsSetA, Is.False);
            Assert.That(result0.B.Key, Is.EqualTo(2));
            Assert.That(result0.B.Value, Is.EqualTo("Two"));

            Assert.That(result2.A.Key, Is.EqualTo(2));
            Assert.That(result2.A.Value, Is.EqualTo("Two"));
            Assert.That(result2.B.Key, Is.EqualTo(2));
            Assert.That(result2.B.Value, Is.EqualTo("Two"));

            Assert.That(result3.A.Key, Is.EqualTo(2));
            Assert.That(result3.A.Value, Is.EqualTo("Two"));
            Assert.That(result3.B.Key, Is.EqualTo(5));
            Assert.That(result3.B.Value, Is.EqualTo("Five"));

            Assert.That(result5.A.Key, Is.EqualTo(5));
            Assert.That(result5.A.Value, Is.EqualTo("Five"));
            Assert.That(result5.B.Key, Is.EqualTo(5));
            Assert.That(result5.B.Value, Is.EqualTo("Five"));

            Assert.That(result6.A.Key, Is.EqualTo(5));
            Assert.That(result6.A.Value, Is.EqualTo("Five"));
            Assert.That(result6.B.Key, Is.EqualTo(8));
            Assert.That(result6.B.Value, Is.EqualTo("Eight"));

            Assert.That(result8.A.Key, Is.EqualTo(8));
            Assert.That(result8.A.Value, Is.EqualTo("Eight"));
            Assert.That(result8.B.Key, Is.EqualTo(8));
            Assert.That(result8.B.Value, Is.EqualTo("Eight"));

            Assert.That(result9.A.Key, Is.EqualTo(8));
            Assert.That(result9.A.Value, Is.EqualTo("Eight"));
            Assert.That(result9.IsSetB, Is.False);
        }


        [Test]
        public void EnumerateRangeShouldWorkProperly()
        {
            var tree = new RedBlackTree<String, int> { { "abc", 0 }, { "bab", 1 }, { "bba", 2 }, { "bua", 3 }, { "bvka kapa", 4 }, { "zza", 5 } };

            var left = tree.EnumeratePairsFromLeftToRight("b").ToArray();
            var right = tree.EnumeratePairsFromRightToLeft("b").ToArray();

            Console.WriteLine("Test");
        }

    }
}
