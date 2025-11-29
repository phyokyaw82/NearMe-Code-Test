# My C# Project

## Project Description

This project is a simple C# application for managing books. It uses SQL Server as the database.

---

## Database Setup

To create the required table in your SQL Server database, run the following SQL script:

```sql
CREATE TABLE [dbo].[Book](
    [Id] [uniqueidentifier] NOT NULL,
    [Title] [nvarchar](max) NOT NULL,
    [Author] [nvarchar](max) NOT NULL,
    [PublishedDate] [date] NOT NULL,
    [CategoryId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    IGNORE_DUP_KEY = OFF, 
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON, 
    OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

ALTER TABLE [dbo].[Book] 
ADD DEFAULT (newid()) FOR [Id];
GO
```

This will create a `Book` table with a unique identifier as the primary key.

---

## Connection String Configuration

Update the `appsettings.json` file in your project with your SQL Server connection information. Example:

```json
{
  "ConnectionStrings": {
    "BMS": "Data Source=.\\SQLEXPRESS;Initial Catalog=BMS;User ID=sa;Password=123123;TrustServerCertificate=True;"
  }
}
```

* **Data Source**: your SQL Server instance
* **Initial Catalog**: database name (`BMS` in this example)
* **User ID** and **Password**: your SQL Server credentials

Make sure your SQL Server allows SQL authentication if you are using a username/password.

---

## Running the Project

1. Open the project in Visual Studio.
2. Ensure the database exists and the connection string is configured.
3. Build and run the project.

---

This README ensures anyone can **create the database table and configure the connection string** before running the project.
