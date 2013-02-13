﻿/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Linq;
using NUnit.Framework;
using Google.Maps.Geocoding;

namespace Google.Maps.Test.Integrations
{
	[TestFixture]
	class GeocodingServiceTests
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Google.Maps.Internal.Http.Factory = new Google.Maps.Test.Integrations.HttpGetResponseFromResourceFactory();
		}
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Google.Maps.Internal.Http.Factory = new Internal.Http.HttpGetResponseFactory();
		}

		[Test]
		public void Empty_address()
		{
			// expectations
			var expectedStatus = ServiceResponseStatus.ZeroResults;
			var expectedResultCount = 0;

			// test
			var request = new GeocodingRequest();
			request.Sensor = false;
			request.Address = "";
			var response = GeocodingService.GetResponse(request);

			// asserts
			Assert.AreEqual(expectedStatus, response.Status);
			Assert.AreEqual(expectedResultCount, response.Results.Count());
		}

		[Test]
		public void GetGeocodingForAddress()
		{
			// expectations
			var expectedStatus = ServiceResponseStatus.Ok;
			var expectedResultCount = 1;
			var expectedType = AddressType.StreetAddress;
			var expectedFormattedAddress = "1600 Amphitheatre Parkway, Mountain View, CA 94043, USA";
			var expectedComponentTypes = new AddressType[] { 
				AddressType.StreetNumber, 
				AddressType.Route,
				AddressType.Locality,
				AddressType.AdministrativeAreaLevel1,
				AddressType.AdministrativeAreaLevel2,
				AddressType.AdministrativeAreaLevel3,
				AddressType.Country,
				AddressType.PostalCode,
				AddressType.Political
			};
			double expectedLatitude = 37.42219410;
			double expectedLongitude = -122.08459320;
			var expectedLocationType = LocationType.Rooftop;
			double expectedSouthwestLatitude = 37.42084511970850;
			double expectedSouthwestLongitude = -122.0859421802915;
			double expectedNortheastLatitude = 37.42354308029149;
			double expectedNortheastLongitude = -122.0832442197085;

			// test
			var request = new GeocodingRequest();
			request.Address = "1600 Amphitheatre Parkway Mountain View CA";
			request.Sensor = false;
			var response = GeocodingService.GetResponse(request);

			// asserts
			Assert.AreEqual(expectedStatus, response.Status, "Status");
			Assert.AreEqual(expectedResultCount, response.Results.Length, "ResultCount");
			Assert.AreEqual(expectedType, response.Results.Single().Types.First(), "Type");
			Assert.AreEqual(expectedFormattedAddress, response.Results.Single().FormattedAddress, "FormattedAddress");
			//Assert.IsTrue(
			//    expectedComponentTypes.OrderBy(x => x).SequenceEqual(
			//        response.Results.Single().AddressComponents.SelectMany(y => y.Types).Distinct().OrderBy(z => z)), "Types");
			//Assert.AreEqual(expectedLatitude, response.Results.Single().Geometry.Location.Latitude, "Latitude");
			//Assert.AreEqual(expectedLongitude, response.Results.Single().Geometry.Location.Longitude, "Longitude");
			Assert.AreEqual(expectedLocationType, response.Results.Single().Geometry.LocationType, "LocationType");
			//Assert.AreEqual(expectedSouthwestLatitude, response.Results.Single().Geometry.Viewport.Southwest.Latitude, "Southwest.Latitude");
			//Assert.AreEqual(expectedSouthwestLongitude, response.Results.Single().Geometry.Viewport.Southwest.Longitude, "Southwest.Longitude");
			//Assert.AreEqual(expectedNortheastLatitude, response.Results.Single().Geometry.Viewport.Northeast.Latitude, "Northeast.Latitude");
			//Assert.AreEqual(expectedNortheastLongitude, response.Results.Single().Geometry.Viewport.Northeast.Longitude, "Northeast.Longitude");
		}

		[Test]
		public void GetGeocodingForCoordinates()
		{
			// expectations
			var expectedStatus = ServiceResponseStatus.Ok;
			var expectedResultCount = 9;
			var expectedTypes = new AddressType[] {
				AddressType.StreetAddress,
				AddressType.Locality,
				AddressType.PostalCode,
				AddressType.Sublocality,
				AddressType.AdministrativeAreaLevel2,
				AddressType.AdministrativeAreaLevel1,
				AddressType.Country,
				AddressType.Political
			};
			var expectedFormattedAddress = "277 Bedford Ave, Brooklyn, NY 11211, USA";
			var expectedComponentTypes = new AddressType[] { 
				AddressType.StreetNumber, 
				AddressType.Route,
				AddressType.Locality,
				AddressType.AdministrativeAreaLevel1,
				AddressType.AdministrativeAreaLevel2,
				AddressType.Sublocality,
				AddressType.Country,
				AddressType.PostalCode,
				AddressType.Political
			};
			var expectedLatitude = 40.7142330m;
			var expectedLongitude = -73.9612910m;
			var expectedLocationType = LocationType.Rooftop;
			var expectedSouthwestLatitude = 40.7110854m;
			var expectedSouthwestLongitude = -73.9644386m;
			var expectedNortheastLatitude = 40.7173806m;
			var expectedNortheastLongitude = -73.9581434m;

			// test
			var request = new GeocodingRequest();
			request.Address = new LatLng(40.714224,-73.961452);
			request.Sensor = false;
			var response = GeocodingService.GetResponse(request);

			// asserts
			Assert.AreEqual(expectedStatus, response.Status, "Status");
			Assert.AreEqual(expectedResultCount, response.Results.Length, "ResultCount");
			Assert.IsTrue(
				expectedTypes.OrderBy(x => x).SequenceEqual(
					response.Results.SelectMany(y => y.Types).Distinct().OrderBy(z => z)));
			Assert.AreEqual(expectedFormattedAddress, response.Results.First().FormattedAddress, "FormattedAddress");
			Assert.IsTrue(
				expectedComponentTypes.OrderBy(x => x).SequenceEqual(
					response.Results.First().AddressComponents.SelectMany(y => y.Types).Distinct().OrderBy(z => z)), "Types");
			Assert.AreEqual(expectedLatitude, response.Results.First().Geometry.Location.Latitude, "Latitude");
			Assert.AreEqual(expectedLongitude, response.Results.First().Geometry.Location.Longitude, "Longitude");
			Assert.AreEqual(expectedLocationType, response.Results.First().Geometry.LocationType, "LocationType");
			Assert.AreEqual(expectedSouthwestLatitude, response.Results.First().Geometry.Viewport.Southwest.Latitude, "Southwest.Latitude");
			Assert.AreEqual(expectedSouthwestLongitude, response.Results.First().Geometry.Viewport.Southwest.Longitude, "Southwest.Longitude");
			Assert.AreEqual(expectedNortheastLatitude, response.Results.First().Geometry.Viewport.Northeast.Latitude, "Northeast.Latitude");
			Assert.AreEqual(expectedNortheastLongitude, response.Results.First().Geometry.Viewport.Northeast.Longitude, "Northeast.Longitude");
		}
	}
}
