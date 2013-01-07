﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Mapper.Caching.CacheKeyResolving;
using Glass.Mapper.Caching.ObjectCaching;
using Glass.Mapper.Configuration;
using Glass.Mapper.Pipelines.ObjectConstruction;
using Glass.Mapper.Pipelines.ObjectConstruction.Tasks.ObjectCachingSaver;
using NSubstitute;
using NUnit.Framework;

namespace Glass.Mapper.Tests.Pipelines.ObjectConstruction.Tasks.ObjectCachingSaver
{
    [TestFixture]
    public class ObjectCachingSaverTaskFixture
    {
        private ObjectCachingSaverTask _task;

        [SetUp]
        public void Setup()
        {
            _task = new ObjectCachingSaverTask();
        }


        [Test]
        public void Execute_ObjectCachingResolver_ObjectReturned()
        {
            //Assign
            Type type = typeof(StubClass);
            var glassConfig = Substitute.For<IGlassConfiguration>();
            var service = Substitute.For<IAbstractService>();

            Context.ResolverFactory = Substitute.For<IDependencyResolverFactory>();
            Context.ResolverFactory.GetResolver().Returns(Substitute.For<IDependencyResolver>());
            Context context = Context.Create(glassConfig);

            var cacheConfiguration = Substitute.For<AbstractObjectCacheConfiguration>();

            var cacheKeyResolver = Substitute.For<AbstractCacheKeyResolver<int>>();

            cacheConfiguration.ObjectCache = Substitute.For<AbstractObjectCache<int>>(cacheKeyResolver);

            AbstractTypeCreationContext abstractTypeCreationContext = Substitute.For<AbstractTypeCreationContext>();
            abstractTypeCreationContext.RequestedType.Returns(type);

            var configuration = Substitute.For<AbstractTypeConfiguration>();
            configuration.Type = type;

            var args = new ObjectConstructionArgs(context, abstractTypeCreationContext, configuration, service, cacheConfiguration);

            args.Result = new StubClass();
            cacheKeyResolver.GetKey(args).ReturnsForAnyArgs(Substitute.For<CacheKey<int>>());

            cacheConfiguration.ObjectCache.GetObject(args).ReturnsForAnyArgs(new StubClass());
            cacheConfiguration.ObjectCache.ContansObject(args).ReturnsForAnyArgs(true);

            //Act
            _task.Execute(args);

            //Assert
            Assert.IsFalse(args.IsAborted);
            Assert.IsTrue(cacheConfiguration.ObjectCache.ContansObject(args));
            Assert.IsAssignableFrom(typeof(StubClass), cacheConfiguration.ObjectCache.GetObject(args));
            
        }

        #region Stubs

        public class StubClass
        {

        }

        public interface IStubInterface
        {

        }
        #endregion
    }
}