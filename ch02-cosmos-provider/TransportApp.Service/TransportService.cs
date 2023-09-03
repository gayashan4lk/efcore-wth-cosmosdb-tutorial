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

public class TransportService
{
	#region Setup

	public TransportService(IDbContextFactory<TransportContext> contextFactory, WriteLine writeLine)
	{
		_contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
		_writeLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
	}

	private readonly IDbContextFactory<TransportContext> _contextFactory;
	private readonly WriteLine _writeLine;

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
		var changedItemCount = await SeedDbWithIds();

		if (changedItemCount > 0)
			_writeLine($"result: {changedItemCount}\nSave successful");
		else
			_writeLine("Nothing saved");
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