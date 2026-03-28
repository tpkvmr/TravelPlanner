using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_PointsOfInterest_PointOfInterestId",
                table: "UserActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_Trips_TripId",
                table: "UserActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_Trips_TripId1",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_UserId_PreferenceKey",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_UserId_PreferenceType",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_ActivityType",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_CreatedAt",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_TripId1",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_UserId_CreatedAt",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_Trips_Category_CreatedAt",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_SearchHistories_Query",
                table: "SearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_SearchHistories_SearchedAt",
                table: "SearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_SearchHistories_UserId_SearchedAt",
                table: "SearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_IsAccepted",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_IsViewed",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_UserId_GeneratedAt",
                table: "Recommendations");

            migrationBuilder.DropIndex(
                name: "IX_AnalyticsEvents_EventTime_EventName",
                table: "AnalyticsEvents");

            migrationBuilder.DropIndex(
                name: "IX_AnalyticsEvents_IsSuccess",
                table: "AnalyticsEvents");

            migrationBuilder.DropIndex(
                name: "IX_AnalyticsEvents_SessionId",
                table: "AnalyticsEvents");

            migrationBuilder.DropColumn(
                name: "TripId1",
                table: "UserActivityLogs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PointsOfInterest");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserId",
                table: "UserActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistories_UserId",
                table: "SearchHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_UserId",
                table: "Recommendations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_PointsOfInterest_PointOfInterestId",
                table: "UserActivityLogs",
                column: "PointOfInterestId",
                principalTable: "PointsOfInterest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_Trips_TripId",
                table: "UserActivityLogs",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_PointsOfInterest_PointOfInterestId",
                table: "UserActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserActivityLogs_Trips_TripId",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserActivityLogs_UserId",
                table: "UserActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_SearchHistories_UserId",
                table: "SearchHistories");

            migrationBuilder.DropIndex(
                name: "IX_Recommendations_UserId",
                table: "Recommendations");

            migrationBuilder.AddColumn<int>(
                name: "TripId1",
                table: "UserActivityLogs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PointsOfInterest",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId_PreferenceKey",
                table: "UserPreferences",
                columns: new[] { "UserId", "PreferenceKey" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId_PreferenceType",
                table: "UserPreferences",
                columns: new[] { "UserId", "PreferenceType" });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_ActivityType",
                table: "UserActivityLogs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_CreatedAt",
                table: "UserActivityLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_TripId1",
                table: "UserActivityLogs",
                column: "TripId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserId_CreatedAt",
                table: "UserActivityLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_Category_CreatedAt",
                table: "Trips",
                columns: new[] { "Category", "CreatedAt" });

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
                name: "IX_Recommendations_IsAccepted",
                table: "Recommendations",
                column: "IsAccepted");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_IsViewed",
                table: "Recommendations",
                column: "IsViewed");

            migrationBuilder.CreateIndex(
                name: "IX_Recommendations_UserId_GeneratedAt",
                table: "Recommendations",
                columns: new[] { "UserId", "GeneratedAt" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_PointsOfInterest_PointOfInterestId",
                table: "UserActivityLogs",
                column: "PointOfInterestId",
                principalTable: "PointsOfInterest",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_Trips_TripId",
                table: "UserActivityLogs",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivityLogs_Trips_TripId1",
                table: "UserActivityLogs",
                column: "TripId1",
                principalTable: "Trips",
                principalColumn: "Id");
        }
    }
}
