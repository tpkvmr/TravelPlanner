using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IgnoreMetadataInTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTrip_AspNetUsers_UserId",
                table: "UserTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTrip_Trips_TripId",
                table: "UserTrip");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTrip",
                table: "UserTrip");

            migrationBuilder.RenameTable(
                name: "UserTrip",
                newName: "UserTrips");

            migrationBuilder.RenameIndex(
                name: "IX_UserTrip_TripId",
                table: "UserTrips",
                newName: "IX_UserTrips_TripId");

            migrationBuilder.AddColumn<string>(
                name: "Activities",
                table: "Trips",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Trips",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BestSeason",
                table: "Trips",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookmarkCount",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Trips",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Trips",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Trips",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "Trips",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Trips",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Trips",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Trips",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationDays",
                table: "Trips",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Trips",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Trips",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccessible",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFamilyFriendly",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPetFriendly",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPopular",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastViewedAt",
                table: "Trips",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Trips",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxBudget",
                table: "Trips",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxDurationDays",
                table: "Trips",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinBudget",
                table: "Trips",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinDurationDays",
                table: "Trips",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Trips",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "Trips",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuitableSeasons",
                table: "Trips",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Trips",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Trips",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Trips",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Trips",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PointsOfInterest",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PointsOfInterest",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "PointsOfInterest",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PointsOfInterest",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "PointsOfInterest",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTrips",
                table: "UserTrips",
                columns: new[] { "UserId", "TripId" });

            migrationBuilder.CreateTable(
                name: "AnalyticsEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    EventData = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TripId = table.Column<int>(type: "INTEGER", nullable: true),
                    POIId = table.Column<int>(type: "INTEGER", nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EventTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<double>(type: "REAL", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsSuccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalyticsEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnalyticsEvents_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Recommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    TripId = table.Column<int>(type: "INTEGER", nullable: true),
                    PointOfInterestId = table.Column<int>(type: "INTEGER", nullable: true),
                    RecommendationType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Score = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsViewed = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAccepted = table.Column<bool>(type: "INTEGER", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TripId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recommendations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recommendations_PointsOfInterest_PointOfInterestId",
                        column: x => x.PointOfInterestId,
                        principalTable: "PointsOfInterest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Recommendations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Recommendations_Trips_TripId1",
                        column: x => x.TripId1,
                        principalTable: "Trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SearchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Query = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SearchType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ResultsCount = table.Column<int>(type: "INTEGER", nullable: true),
                    SearchedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Filters = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    LocationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsSuccessful = table.Column<bool>(type: "INTEGER", nullable: false),
                    SelectedResultId = table.Column<int>(type: "INTEGER", nullable: true),
                    SearchEngine = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TripId = table.Column<int>(type: "INTEGER", nullable: true),
                    PointOfInterestId = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TripId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_PointsOfInterest_PointOfInterestId",
                        column: x => x.PointOfInterestId,
                        principalTable: "PointsOfInterest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_Trips_TripId1",
                        column: x => x.TripId1,
                        principalTable: "Trips",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PreferenceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PreferenceKey = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PreferenceValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Category",
                table: "Trips",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Category_CreatedAt",
                table: "Trips",
                columns: new[] { "Category", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CreatedAt",
                table: "Trips",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Location",
                table: "Trips",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_PointsOfInterest_Category",
                table: "PointsOfInterest",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_PointsOfInterest_Latitude_Longitude",
                table: "PointsOfInterest",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_EventTime_EventName",
                table: "AnalyticsEvents",
                columns: new[] { "EventTime", "EventName" });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_IsSuccess",
                table: "AnalyticsEvents",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_SessionId",
                table: "AnalyticsEvents",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_TripId",
                table: "AnalyticsEvents",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsEvents_UserId",
                table: "AnalyticsEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_IsAccepted",
                table: "Recommendations",
                column: "IsAccepted");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_IsViewed",
                table: "Recommendations",
                column: "IsViewed");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_PointOfInterestId",
                table: "Recommendations",
                column: "PointOfInterestId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_TripId",
                table: "Recommendations",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_TripId1",
                table: "Recommendations",
                column: "TripId1");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_UserId_GeneratedAt",
                table: "Recommendations",
                columns: new[] { "UserId", "GeneratedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_Query",
                table: "SearchHistories",
                column: "Query");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_SearchedAt",
                table: "SearchHistories",
                column: "SearchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_UserId_SearchedAt",
                table: "SearchHistories",
                columns: new[] { "UserId", "SearchedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_ActivityType",
                table: "UserActivityLogs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_CreatedAt",
                table: "UserActivityLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_PointOfInterestId",
                table: "UserActivityLogs",
                column: "PointOfInterestId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_TripId",
                table: "UserActivityLogs",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_TripId1",
                table: "UserActivityLogs",
                column: "TripId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserId_CreatedAt",
                table: "UserActivityLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId_PreferenceKey",
                table: "UserPreferences",
                columns: new[] { "UserId", "PreferenceKey" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId_PreferenceType",
                table: "UserPreferences",
                columns: new[] { "UserId", "PreferenceType" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTrips_AspNetUsers_UserId",
                table: "UserTrips",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTrips_Trips_TripId",
                table: "UserTrips",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTrips_AspNetUsers_UserId",
                table: "UserTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTrips_Trips_TripId",
                table: "UserTrips");

            migrationBuilder.DropTable(
                name: "AnalyticsEvents");

            migrationBuilder.DropTable(
                name: "Recommendations");

            migrationBuilder.DropTable(
                name: "SearchHistories");

            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Category",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Category_CreatedAt",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_CreatedAt",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Location",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_PointsOfInterest_Category",
                table: "PointsOfInterest");

            migrationBuilder.DropIndex(
                name: "IX_PointsOfInterest_Latitude_Longitude",
                table: "PointsOfInterest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTrips",
                table: "UserTrips");

            migrationBuilder.DropColumn(
                name: "Activities",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "BestSeason",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "BookmarkCount",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DurationDays",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsAccessible",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsFamilyFriendly",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsPetFriendly",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsPopular",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "LastViewedAt",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "MaxBudget",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "MaxDurationDays",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "MinBudget",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "MinDurationDays",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "SuitableSeasons",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "UserTrips",
                newName: "UserTrip");

            migrationBuilder.RenameIndex(
                name: "IX_UserTrips_TripId",
                table: "UserTrip",
                newName: "IX_UserTrip_TripId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTrip",
                table: "UserTrip",
                columns: new[] { "UserId", "TripId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTrip_AspNetUsers_UserId",
                table: "UserTrip",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTrip_Trips_TripId",
                table: "UserTrip",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
