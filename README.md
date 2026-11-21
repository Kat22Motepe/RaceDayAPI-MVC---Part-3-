RaceDay – Event Management System

RaceDay is a comprehensive, full-stack web application designed to manage road running, walking, and cycling events in South Africa. The project is built progressively across three parts and uses a modern, decoupled architecture. The solution consists of five separate projects: an API, an MVC front-end, a Domain layer, an Infrastructure layer, and an API Tests project. This separation ensures that the MVC application does not directly access the database, instead relying entirely on RESTful API calls via `IHttpClientFactory`. The system supports two distinct roles: Organisers, who can create and manage events, categories, enrolments, and results, and Participants, who can browse events, enrol, and track their personal race history. User authentication is handled through a custom session-based token system, with passwords securely hashed and salted in the database. 

One of the key features of Part 3 is the integration of Azure Blob Storage (using the local Azurite emulator). When an Organiser creates or edits an event, they can upload a banner image. This image is sent from the MVC application to the API, which then uploads it to an Azurite container and saves the resulting Blob URL in the database. The same flow applies to Participant profile pictures. The MVC application is fully containerised using Docker, allowing the entire solution to be run with a single command. The front-end features a responsive, motorsport-themed Bootstrap interface with role-based navigation, colour-coded enrolment statuses, and graceful input validation error handling.

 Setup and Run Instructions

To run this project locally, you must first ensure that the Azurite storage emulator is running. You can start Azurite via the command line by running `azurite`, or by locating and running the `azurite.exe` executable installed with Visual Studio 2022. Once Azurite is running, open the `RaceDay.sln` solution in Visual Studio. Because the MVC application depends on the API, you must start the API project first. Right-click the `RaceDay.API` project and select **Debug > Start New Instance**. Once the Swagger interface loads in your browser, right-click the `RaceDay.MVC` project and select **Debug > Start New Instance**. You can then register a new account as an Organiser or Participant and log in to explore the system.

To run the application using Docker, you can use the provided `docker-compose.yml` file. Open a terminal in the root directory of the solution and run the following command:

```bash
docker-compose up --build
```

This will build the Docker images for both the API and the MVC application and start the containers. You can access the MVC front-end by navigating to `http://localhost:8080` in your browser. The API will be available at `http://localhost:8080` (or the port specified in your compose file).

 CI/CD and Video Demonstration

The project is fully integrated with GitHub Actions. The CI/CD pipeline is configured to restore, build, and run all unit tests for both the API and MVC projects on every push to the repository. 

A complete video walkthrough demonstrating the application's functionality, code structure, Azurite image uploads, and Docker container execution can be viewed at the following unlisted YouTube link:

YouTube Video: [INSERT YOUR UNLISTED YOUTUBE LINK HERE]

 AI Disclosure

AI tools (Gemini) were used during the development of this project for architectural guidance, troubleshooting, and boilerplate code generation. The full transcript of the AI interactions can be found at the following link:

**AI Chat Transcript:** [INSERT LINK TO THIS CHAT OR YOUR AI TRANSCRIPT HERE]
