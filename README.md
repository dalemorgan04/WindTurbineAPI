# Wind Turbine API - Summary

This project provides an API for managing wind turbine sensor data

## Technologies Used:

* **ASP.NET Core:** Web framework for building the API
* **C#:** Programming language
* **Entity Framework Core:** ORM for database interaction (SQLite)
* **AutoMapper:** Object-object mapper for DTO to Entity conversions
* **Swagger** Library for generating Swagger API documentation
* **SQLite:** Lightweight, file-based relational database

## Project Layers:

1.  **Domain:**
    * Contains core business entities (e.g., `Sensor`, `SensorRecord`)
    * Includes value objects (e.g., `ReadingValue`)
    * Defines interfaces for repositories (`ISensorRepository`, `ISensorRecordRepository`) and domain services (`ISensorFactory`)

2.  **Application:**
    * Contains DTOs (Data Transfer Objects) for request and response payloads (e.g., `CreateSensorDto`, `SensorRecordDto`)
    * Defines interfaces for application services (`ISensorService`, `ISensorRecordService`).
    * Implements application services (`SensorService`, `SensorRecordService`) containing business logic and orchestrating domain and infrastructure layers
    * Includes AutoMapper profiles for mapping between DTOs and Entities

3.  **Infrastructure:**
    * Implements repository interfaces using Entity Framework Core (`SensorRepository`, `SensorRecordRepository`)
    * Configures the `ApplicationDbContext` for database interaction
    * Implements the `IUnitOfWork` pattern for managing database transactions

4.  **API (Presentation):**
    * Contains ASP.NET Core controllers (`SensorsController`, `SensorRecordController`) that handle HTTP requests and responses
    * Uses dependency injection to access application services and `IMapper`
    * Utilizes Swagger attributes and XML comments for API documentation
    * Handles request validation and response formatting

## Key Features:

* CRUD operations for sensors
* Creation and retrieval of sensor records
* Filtering of sensor records by sensor name, date range, and temperature values
* API documentation using Swagger UI
* Use of DTOs for data transfer
* Mapping between DTOs and Entities using AutoMapper
* Unit of Work pattern for database changes
* Asynchronous operations throughout the application