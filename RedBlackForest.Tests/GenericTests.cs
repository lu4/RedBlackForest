﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var result1 = tree.AddOrGet(2, "Two");

            var result2 = tree[2];

            Assert.That(result1.Value, Is.EqualTo("Two"));
            Assert.That(result2, Is.EqualTo("Two"));
        }

        [Test]
        public void AddOrGetShouldGetWhenValueAlreadyExists()
        {
            var tree = new RedBlackTree<Int32, String> { { 0, "Zero" }, { 1, "One" } };

            var result1 = tree.AddOrGet(0, "Two");

            var result2 = tree[0];

            Assert.That(result1.Value, Is.EqualTo("Zero"));
            Assert.That(result2, Is.EqualTo("Zero"));
        }

        [Test]
        public void NearestNodesShouldReturnProperValues()
        {
            var tree = new RedBlackTree<Int32, String>();

            var resultZ = tree.NearestNodes(5);

            tree.Add(2, "Two");
            tree.Add(5, "Five");
            tree.Add(8, "Eight");

            var result0 = tree.NearestNodes(0);
            var result2 = tree.NearestNodes(2);
            var result3 = tree.NearestNodes(3);
            var result5 = tree.NearestNodes(5);
            var result6 = tree.NearestNodes(6);
            var result8 = tree.NearestNodes(8);
            var result9 = tree.NearestNodes(9);

            Assert.That(resultZ.A, Is.Null);
            Assert.That(resultZ.B, Is.Null);

            Assert.That(result0.A, Is.Null);
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
            Assert.That(result9.B, Is.Null);
        }
    }
}