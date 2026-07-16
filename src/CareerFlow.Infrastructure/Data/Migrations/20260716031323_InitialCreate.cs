using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    type = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    content = table.Column<string>(type: "jsonb", maxLength: 500, nullable: false),
                    headers = table.Column<string>(type: "jsonb", maxLength: 500, nullable: false, defaultValue: "{}"),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 500, nullable: false, defaultValue: "pending"),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    max_retries = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    last_error = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    error_stack_trace = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    processed_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    sent_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    email = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(300)", maxLength: 500, nullable: false),
                    email_verified_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    last_password_change_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    locked_until = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    two_factor_secret = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    role = table.Column<string>(type: "varchar(30)", nullable: false, defaultValue: "User"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_premium = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    premium_until = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    notification_preferences = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true, defaultValue: "{}"),
                    theme_preferences = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true, defaultValue: "{}"),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: false),
                    entity_type = table.Column<string>(type: "varchar(50)", maxLength: 500, nullable: true),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_values = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true),
                    new_values = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true),
                    details = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true, defaultValue: "{}"),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    state = table.Column<string>(type: "varchar(2)", maxLength: 500, nullable: true),
                    birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    professional_summary = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    photo_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    current_position = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    current_company = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    resume_slug = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_persons_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "varchar(300)", maxLength: 500, nullable: false),
                    issuer = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    issue_date = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "date", nullable: true),
                    certificate_id = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    credential_id = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    credential_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certificates_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "educations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    institution = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    course = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    education_level = table.Column<string>(type: "varchar(50)", nullable: false),
                    status = table.Column<string>(type: "varchar(30)", nullable: false, defaultValue: "Completed"),
                    start_date = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_current = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    description = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    grade = table.Column<string>(type: "varchar(20)", maxLength: 500, nullable: true),
                    thesis_title = table.Column<string>(type: "varchar(300)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_educations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_educations_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "experiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_name = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    position = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_current = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    description = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    skills_used = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    city = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    state = table.Column<string>(type: "varchar(2)", maxLength: 500, nullable: true),
                    country = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: false, defaultValue: "Brasil"),
                    employment_type = table.Column<string>(type: "varchar(30)", nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_experiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_experiences_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_name = table.Column<string>(type: "varchar(50)", maxLength: 500, nullable: false),
                    proficiency_level = table.Column<string>(type: "varchar(30)", nullable: false),
                    is_native = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    reading_level = table.Column<string>(type: "varchar(30)", maxLength: 500, nullable: true),
                    writing_level = table.Column<string>(type: "varchar(30)", maxLength: 500, nullable: true),
                    listening_level = table.Column<string>(type: "varchar(30)", maxLength: 500, nullable: true),
                    speaking_level = table.Column<string>(type: "varchar(30)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_languages_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resume_analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_views = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    unique_views = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    pdf_downloads = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    shares_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ats_score = table.Column<int>(type: "integer", nullable: true),
                    ats_compatibility = table.Column<int>(type: "integer", nullable: true),
                    ats_issues = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    ats_suggestions = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    average_view_duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    last_viewed_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    detected_keywords = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true),
                    missing_keywords = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true),
                    analyzed_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    status = table.Column<string>(type: "varchar(30)", nullable: false, defaultValue: "Draft"),
                    published_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resume_analytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resume_analytics_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resume_suggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 500, nullable: false),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    priority = table.Column<string>(type: "varchar(30)", maxLength: 500, nullable: false, defaultValue: "medium"),
                    is_applied = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    applied_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resume_suggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resume_suggestions_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resume_views",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", maxLength: 500, nullable: true),
                    referrer = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    viewer_country = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    viewer_city = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: true),
                    source = table.Column<string>(type: "varchar(50)", maxLength: 500, nullable: true),
                    view_duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    pdf_downloaded = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resume_views", x => x.Id);
                    table.ForeignKey(
                        name: "FK_resume_views_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 500, nullable: false),
                    category = table.Column<string>(type: "varchar(50)", nullable: false),
                    proficiency_level = table.Column<string>(type: "varchar(30)", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_skills_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "social_networks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    network_type = table.Column<string>(type: "varchar(30)", nullable: false),
                    url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_social_networks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_social_networks_persons_person_id",
                        column: x => x.person_id,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_activity_logs_action",
                table: "activity_logs",
                columns: new[] { "user_id", "action" });

            migrationBuilder.CreateIndex(
                name: "idx_activity_logs_user",
                table: "activity_logs",
                columns: new[] { "user_id", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_activity_logs_CreatedAt",
                table: "activity_logs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_certificates_issuer",
                table: "certificates",
                columns: new[] { "person_id", "issuer" });

            migrationBuilder.CreateIndex(
                name: "idx_certificates_person",
                table: "certificates",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_certificates_CreatedAt",
                table: "certificates",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_educations_current",
                table: "educations",
                columns: new[] { "person_id", "is_current" });

            migrationBuilder.CreateIndex(
                name: "idx_educations_level",
                table: "educations",
                columns: new[] { "person_id", "education_level" });

            migrationBuilder.CreateIndex(
                name: "idx_educations_person",
                table: "educations",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_educations_CreatedAt",
                table: "educations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_experiences_current",
                table: "experiences",
                columns: new[] { "person_id", "is_current" });

            migrationBuilder.CreateIndex(
                name: "idx_experiences_dates",
                table: "experiences",
                columns: new[] { "person_id", "start_date" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_experiences_person",
                table: "experiences",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_experiences_CreatedAt",
                table: "experiences",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_languages_person",
                table: "languages",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "idx_languages_unique",
                table: "languages",
                columns: new[] { "person_id", "language_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_languages_CreatedAt",
                table: "languages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_outbox_created",
                table: "outbox_messages",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_outbox_pending",
                table: "outbox_messages",
                columns: new[] { "status", "created_at" },
                filter: "status = 'pending'");

            migrationBuilder.CreateIndex(
                name: "idx_outbox_type",
                table: "outbox_messages",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "idx_persons_public",
                table: "persons",
                column: "is_public");

            migrationBuilder.CreateIndex(
                name: "idx_persons_slug",
                table: "persons",
                column: "resume_slug",
                unique: true,
                filter: "resume_slug IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_persons_user",
                table: "persons",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_persons_CreatedAt",
                table: "persons",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_resume_analytics_person",
                table: "resume_analytics",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resume_analytics_CreatedAt",
                table: "resume_analytics",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_resume_suggestions_person",
                table: "resume_suggestions",
                columns: new[] { "person_id", "is_applied" });

            migrationBuilder.CreateIndex(
                name: "IX_resume_suggestions_CreatedAt",
                table: "resume_suggestions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_resume_views_person",
                table: "resume_views",
                columns: new[] { "person_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_resume_views_source",
                table: "resume_views",
                columns: new[] { "person_id", "source" });

            migrationBuilder.CreateIndex(
                name: "IX_resume_views_created_at",
                table: "resume_views",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_skills_category",
                table: "skills",
                columns: new[] { "person_id", "category" });

            migrationBuilder.CreateIndex(
                name: "idx_skills_level",
                table: "skills",
                columns: new[] { "person_id", "proficiency_level" });

            migrationBuilder.CreateIndex(
                name: "idx_skills_person",
                table: "skills",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_skills_CreatedAt",
                table: "skills",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_social_networks_person",
                table: "social_networks",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "idx_social_networks_unique",
                table: "social_networks",
                columns: new[] { "person_id", "network_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_social_networks_CreatedAt",
                table: "social_networks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_users_login",
                table: "users",
                column: "email",
                unique: true,
                filter: "deleted_at IS NULL")
                .Annotation("Npgsql:IndexInclude", new[] { "Id", "password_hash", "is_active", "locked_until" });

            migrationBuilder.CreateIndex(
                name: "idx_users_premium",
                table: "users",
                columns: new[] { "is_premium", "premium_until" });

            migrationBuilder.CreateIndex(
                name: "idx_users_role",
                table: "users",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_users_CreatedAt",
                table: "users",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_logs");

            migrationBuilder.DropTable(
                name: "certificates");

            migrationBuilder.DropTable(
                name: "educations");

            migrationBuilder.DropTable(
                name: "experiences");

            migrationBuilder.DropTable(
                name: "languages");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "resume_analytics");

            migrationBuilder.DropTable(
                name: "resume_suggestions");

            migrationBuilder.DropTable(
                name: "resume_views");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "social_networks");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
