# Car Workshop Management Application

## Overview

This project is a web-based application for managing a car workshop, developed using Microsoft ASP.NET Core MVC. The application allows for efficient management of employees, tickets, and parts within a car workshop environment. The project was developed as part of the Graphical User Interfaces (EGUI) course at the Computer Science Institute, with a focus on implementing robust CRUD operations, user authentication, and detailed entity management.

## Features

### Employee Management
- **Add/Remove Employees**: Admin user can add or remove employees.
- **Employee Calendar**: Each employee has a calendar displaying their busy and available time slots.
- **Hourly Rate**: Track the hourly rate of each employee.

### Ticket Management
- **Manage Calendar**: Employees can manage their workshop calendar by adding or removing hourly slots on given days.
- **View Slots**: Employees can see available and occupied slots for repairs.
- **Accept Tickets**: Employees can accept repair tickets, specifying their slots for the given ticket. The ticket's status changes from 'created' to 'in progress'.
- **Ticket States**: Tickets can change states (created, in progress, done, closed).

### Creating a Ticket
- **Ticket Details**: Create a ticket with details including the brand, model, registration ID of the vehicle, and a textual description of the problem.

### Maintaining a Ticket
- **Repair Estimate**: Add or update repair estimates, including description and expected cost.
- **Client Approval**: Indicate whether the client has accepted the estimate.
- **Parts Management**: Track parts used in the repair, including name, description, amount, unit price, and total price (calculated automatically).
- **Client Payment**: Record the price paid by the client.

## Setup Instructions

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later)
- [Visual Studio](https://visualstudio.microsoft.com/) (with ASP.NET and web development workload)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)

### Installation
1. **Clone the repository:**
    ```bash
    git clone https://github.com/m-Jakub/Car-Workshop-with-MVC.git
    cd Car-Workshop-with-MVC
    ```

2. **Setup the Database:**
   - Update the `appsettings.json` file with your SQL Server connection string.
   - Run the following commands to create and seed the database:
    ```bash
    dotnet ef database update
    ```

3. **Run the Application:**
    ```bash
    dotnet run
    ```
   Open your browser and navigate to `https://localhost:7133` to view the application.

### Usage
- **Admin User**: Use the admin interface to manage employees and view reports.
- **Employee User**: Employees can log in to manage their calendars, accept tickets, and update ticket details.

### Project Structure
- **Controllers**: Handle HTTP requests and responses.
- **Models**: Define the data structures and relationships.
- **Views**: HTML templates for displaying data to the user.
- **Data**: Database context and migrations.

## Screenshots
![login](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/778012f3-b354-421c-a020-978c8ee15378)
![employee_management](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/44230a45-889e-465a-aaf9-d95af0d9eb62)
![ticket_management](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/24c24bb0-5e20-45a7-8f60-bd8c7d2bac2d)
![creating_ticket](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/0cc5c472-47b8-40c0-bbe2-ad96c073b2a6)
![calendar](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/62fc2992-84ea-47ce-98d2-42166dbb8a21)
![accepting_ticket](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/4f896a26-3ae7-47ab-9877-57e90c83b5eb)
![choosing_slots](https://github.com/m-Jakub/Car-Workshop-with-MVC/assets/133584402/46146d79-0513-46c6-ae87-2d9f9e681c58)

