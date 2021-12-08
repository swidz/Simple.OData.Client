﻿using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Simple.OData.Client.Tests.FluentApi
{
	public class InsertDynamicTests : TestBase
	{
		[Fact]
		public async Task Insert()
		{
			var client = new ODataClient(CreateDefaultSettings().WithHttpMock());
			var x = ODataDynamic.Expression;
			var product = await client
				.For(x.Products)
				.Set(x.ProductName = "Test1", x.UnitPrice = 18m)
				.InsertEntryAsync();

			Assert.Equal("Test1", product.ProductName);
		}

		[Fact]
		public async Task InsertAutogeneratedID()
		{
			var client = new ODataClient(CreateDefaultSettings().WithHttpMock());
			var x = ODataDynamic.Expression;
			var product = await client
				.For(x.Products)
				.Set(x.ProductName = "Test1", x.UnitPrice = 18m)
				.InsertEntryAsync();

			Assert.True((int)product.ProductID > 0);
			Assert.Equal("Test1", product.ProductName);
		}

		[Fact]
		public async Task InsertExpando()
		{
			var client = new ODataClient(CreateDefaultSettings().WithHttpMock());
			var x = ODataDynamic.Expression;
			dynamic expando = new ExpandoObject();
			expando.ProductName = "Test9";
			expando.UnitPrice = 18m;

			var product = await client
				.For(x.Products)
				.Set(expando)
				.InsertEntryAsync();

			Assert.True((int)product["ProductID"] > 0);
		}

		[Fact]
		public async Task InsertProductWithCategoryByID()
		{
			var client = new ODataClient(CreateDefaultSettings().WithHttpMock());
			var x = ODataDynamic.Expression;
			var category = await client
				.For(x.Categories)
				.Set(x.CategoryName = "Test3")
				.InsertEntryAsync();
			var product = await client
				.For(x.Products)
				.Set(x.ProductName = "Test4", x.UnitPrice = 18m, x.CategoryID = category.CategoryID)
				.InsertEntryAsync();

			Assert.Equal("Test4", product.ProductName);
			Assert.Equal(category.CategoryID, product.CategoryID);
			category = await client
				.For(x.Categories)
				.Expand(x.Products)
				.Filter(x.CategoryName == "Test3")
				.FindEntryAsync();
			Assert.True((category.Products as IEnumerable<dynamic>).Count() == 1);
		}

		[Fact]
		public async Task InsertProductWithCategoryByAssociation()
		{
			var client = new ODataClient(CreateDefaultSettings().WithHttpMock());
			var x = ODataDynamic.Expression;
			var category = await client
				.For(x.Categories)
				.Set(x.CategoryName = "Test5")
				.InsertEntryAsync();
			var product = await client
				.For(x.Products)
				.Set(x.ProductName = "Test6", x.UnitPrice = 18m, x.Category = category)
				.InsertEntryAsync();

			Assert.Equal("Test6", product.ProductName);
			Assert.Equal(category.CategoryID, product.CategoryID);
			category = await client
				.For(x.Categories)
				.Expand(x.Products)
				.Filter(x.CategoryName == "Test5")
				.FindEntryAsync();
			Assert.True((category.Products as IEnumerable<dynamic>).Count() == 1);
		}

		[Fact]
		public async Task InsertShip()
		{
			var client = new ODataClient(CreateDefaultSettings().WithHttpMock());
			var x = ODataDynamic.Expression;
			var ship = await client
				.For(x.Transport)
				.As(x.Ship)
				.Set(x.ShipName = "Test1")
				.InsertEntryAsync();

			Assert.Equal("Test1", ship.ShipName);
		}
	}
}
