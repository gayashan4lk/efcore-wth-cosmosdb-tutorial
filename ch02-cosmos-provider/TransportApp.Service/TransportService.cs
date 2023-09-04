#region Info and license

/*
  This demo application accompanies Pluralsight course 'Using EF Core 6 with Azure Cosmos DB', 
  by Jurgen Kevelaers. See https://pluralsight.pxf.io/efcore6-cosmos.

  MIT License

  Copyright (c) 2022 Jurgen Kevelaers

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

#endregion

using Microsoft.EntityFrameworkCore;
using TransportApp.Data;
using TransportApp.Domain;

namespace TransportApp.Service;

public delegate void WriteLine(string text = "", bool highlight = false, bool isException = false);
public delegate void WaitForNext(string actionName);

public class TransportService
{
	#region Setup

	public TransportService(IDbContextFactory<TransportContext> contextFactory, WriteLine writeLine, WaitForNext waitForNext)
	{
		_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		_writeLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
		_waitForNext = waitForNext ?? throw new ArgumentNullException(nameof(waitForNext));
	}

	private readonly IDbContextFactory<TransportContext> _contextFactory;
	private readonly WriteLine _writeLine;
	private readonly WaitForNext _waitForNext;

	private async Task RecreateDatabase()
	{
		await using var context = await _contextFactory.CreateDbContextAsync();
		await context.Database.EnsureDeletedAsync();
		await context.Database.EnsureCreatedAsync();
	}

	#endregion

	public async Task RunSample()
	{
		await RecreateDatabase();
		_writeLine();
		_writeLine("Adding items...");

		/*var changedItemCount = await SeedDbWithIds();

		if (changedItemCount > 0)
			_writeLine($"result: {changedItemCount}\nSave successful");
		else
			_writeLine("Nothing saved");*/

		using var defaultContext = await _contextFactory.CreateDbContextAsync();

		var itemCount= await AddItemsFromDefaultContext(defaultContext);
		_writeLine($"Item count: {itemCount}");
	}

	private async Task<int> AddItemsFromDefaultContext(TransportContext defaultContext)
	{
		var vehicle = new Vehicle
		{
			Make = "Toyota",
			Model = "Hilux",
			Year = 2005,
			LicensePlate = "JKI2563",
			Mileage = 25300,
			PassengerSeatCount = 5,
			TechnicalSpecifications = new Dictionary<string, string>
			{
				{
					"Maximum horsepower", "275"
				},
				{
					"Maximum torque", "265"
				},
				{
					"Fuel capacity", "25.1"
				},
				{
					"Length", "219.9"
				},
				{
					"Width", "81.3"
				}
			},
			CheckupUtcs = new List<DateTime>
			{
				new DateTime(2019, 2, 12, 11, 0, 0, DateTimeKind.Utc),
				new DateTime(2020, 2, 12, 11, 0, 0, DateTimeKind.Utc),
				new DateTime(2021, 2, 12, 11, 0, 0, DateTimeKind.Utc),
				new DateTime(2022, 2, 12, 11, 0, 0, DateTimeKind.Utc),
				new DateTime(2023, 2, 12, 11, 0, 0, DateTimeKind.Utc),
			}
		};

		var driverAddress = new Address
		{
			City = "Galle",
			State = "Sothern",
			Street = "Barckly road",
			HouseNumber = "25/A",
		};

		var originAddress = new Address
		{
			City = "Salt Lake City",
			State = "Utah",
			Street = "Course road",
			HouseNumber = "54254/E/T"
		};

		var destinationAddress = new Address
		{
			City = "Rock springs",
			State = "Wyoming",
			Street = "Canal lane",
			HouseNumber = "540/Z/Z"
		};

		var driver = new Driver
		{
			FirstName = "Jhonny",
			LastName = "Depp",
			EmploymentBeginUtc = new DateTime(2022, 1, 14, 9, 0, 0, DateTimeKind.Utc),
			Address = driverAddress
		};

		var trip = new Trip
		{
			BeginUtc = new DateTime(2022, 2, 23, 10, 45, 12, DateTimeKind.Utc),
			EndUtc = new DateTime(2022, 2, 23, 11, 25, 36, DateTimeKind.Utc),
			PassengerCount = 4,
			// DriverId = driver.DriverId,
			// VehicleId = vehicle.VehicleId,
			Driver = driver,
			Vehicle = vehicle,
			FromAddress = originAddress,
			ToAddress = destinationAddress,
		};

		defaultContext.Add(vehicle);
		defaultContext.Add(driver);
		defaultContext.Add(trip);

		WriteTripInfo(trip);

		return await defaultContext.SaveChangesAsync();
	}

	private void WriteTripInfo(Trip trip)
	{
		_writeLine($"From address instance available on trip: {(trip.FromAddress == null ? "no" : "yes")}");
		_writeLine($"To address instance available on trip: {(trip.ToAddress == null ? "no" : "yes")}");
		_writeLine($"Driver instance available on trip: {(trip.Driver == null ? "no" : "yes")}");
		_writeLine($"Vehicle instance available on trip: {(trip.Vehicle == null ? "no" : "yes")}");

		if (trip.Driver?.Trips != null) _writeLine($"Driver has {trip.Driver.Trips.Count} trip instance(s).");
		if (trip.Vehicle?.Trips != null) _writeLine($"Vehicle has {trip.Vehicle.Trips.Count} trip instance(s).");
	}

	private async Task<int> SeedDbWithIds()
	{
		var context = await _contextFactory.CreateDbContextAsync();

		context.Add(
			new Address
			{
				// AddressId = $"{nameof(Address)}-1",
				City = "Colombo",
				State = "Western",
				Street = "Central St.",
				HouseNumber = "999"
			});

		context.Add(
			new Driver
			{
				// DriverId = $"{nameof(Driver)}-1",
				FirstName = "Jake",
				LastName = "Sully",
				EmploymentBeginUtc = DateTime.UtcNow
			});

		context.Add(
			new Vehicle
			{
				// VehicleId = $"{nameof(Vehicle)}-1",
				Make = "Toyota",
				Model = "Land Cruiser",
				Year = 1998,
				LicensePlate = "2SDG586",
				Mileage = 150336,
				PassengerSeatCount = 6
			});

		context.Add(
			new Trip
			{
				// TripId = $"{nameof(Trip)}-1",
				BeginUtc = new DateTime(2023, 3, 25, 11, 25, 0, DateTimeKind.Utc),
				EndUtc = DateTime.UtcNow,
				PassengerCount = 4
			});

		return await context.SaveChangesAsync();
	}

	private async Task<int> SeedDbWithGuidIds()
	{
		var context = await _contextFactory.CreateDbContextAsync();

		context.Add(
			new Address
			{
				// AddressId = new Guid().ToString(),
				City = "Colombo",
				State = "Western",
				Street = "Central St.",
				HouseNumber = "999"
			});

		context.Add(
			new Driver
			{
				// DriverId = new Guid().ToString(),
				FirstName = "Jake",
				LastName = "Sully",
				EmploymentBeginUtc = DateTime.UtcNow
			});

		context.Add(
			new Vehicle
			{
				// VehicleId = new Guid().ToString(),
				Make = "Toyota",
				Model = "Land Cruiser",
				Year = 1998,
				LicensePlate = "2SDG586",
				Mileage = 150336,
				PassengerSeatCount = 6
			});

		context.Add(
			new Trip
			{
				// TripId = new Guid().ToString(),
				BeginUtc = new DateTime(2023, 3, 25, 11, 25, 0, DateTimeKind.Utc),
				EndUtc = DateTime.UtcNow,
				PassengerCount = 4
			});

		return await context.SaveChangesAsync();
	}
}