## Project Name 
Trainee Management System

## Technology Used
Asp.net core, MySql, Redis, RabbitMQ & Docker

## Workflow Diagram
<img width="1002" height="645" alt="Screenshot 2026-06-29 192131" src="https://github.com/user-attachments/assets/93e9fed9-8086-44ff-a616-52fc8ec07f8d" />

## Backend setup steps

  ### TraineeManagement.Api
  1. Navigate to the TraineeManagement.Api folder  
    From the root of the project run:  
    `cd TraineeManagement.Api`

  2. Set up .env files  
    Run the following command and enter appropriate values for all the environment variables  
    `cp .env.example .env `

  3. Restore dependencies  
    `dotnet restore` 

  4. Build the project   
    `dotnet build`

  6. Run the application  
    `dotnet run`

  ### SubmissionProcesso.Worker
  1. Navigate to the SubmissionProcesso.Worker folder    
    From the root of the project run:   
    `cd SubmissionProcesso.Worker`

  2. Set up .env files    
    Run the following command and enter appropriate values for all the environment variables  
    `cp .env.example .env `

  3. Restore dependencies  
    `dotnet restore` 

  4. Build the project   
    `dotnet build`

  5. Run database migrations  
    `dotnet ef database update`

  6. Run the application  
    `dotnet run`

## MySQL setup steps
1. Get a database connection string

2. Add Database connection string in the .env file of TraineeManagement.Api and SubmissionProcessor.Worker  
  `ConnectionStrings__DefaultConnection=Your-Database-Connection-String`

3. Run the following command in the root of the project dir to make sure there are no errors.  
  `dotnet build`

4. Run the following command in the root of the project dir to create tables in the database   
  `dotnet ef database update -p Shared -s TraineeManagement.Api`

Once ran successfully, the API and the database are in sync. We can test the connection by using swagger UI, try adding one entry   using POST end point and see if it is shown in the datase or not.

## Redis setup steps
1. Get a redis connection string and ensure your redis instance is up and running

2. Add Redis connection string in the .env file of TraineeManagement.Api and SubmissionProcessor.Worker  
  `ConnectionStrings__Redis="Your-Redis-Connection-String"`

## Rabitmq setup
1. Ensure a rabbitMQ instance is up and running

2. Add RabbitMQ username in the .env file of TraineeManagement.Api and SubmissionProcessor.Worker  
  `dotnet user-secrets set "RabbitMQ:UserName" "Your_Username"`

3. Add RabbitMQ password in the .env file of TraineeManagement.Api and SubmissionProcessor.Worker  
  `dotnet user-secrets set "RabbitMQ:Password" "your_password"`


## Login credentials for testing
``` javascript
{
  "username": "michael",
  "password": "pass"
}
```

## JWT usage instructions
1. Initialise User Secrets
  `dotnet user-secrets init`

2. Add Jwt Key to secrets
  `dotnet user-secrets set "Jwt:Key" "Your-Jwt-Key"`

3. Run the following command to view you secret
  `dotnet user-secrets list`

4. Run the following command to make sure there are no errors.
  `dotnet build`

## API List
- GET /api/health

- POST   /api/auth/login 

- GET     /api/trainees
- GET     /api/trainees/{id}
- POST    /api/trainees
- PUT     /api/trainees/{id}
- DELETE  /api/trainees/{id}

- GET    /api/mentors 
- GET    /api/mentors/{id} 
- POST   /api/mentors 
- PUT    /api/mentors/{id} 
- DELETE /api/mentors/{id} 

- GET    /api/learning-tasks 
- GET    /api/learning-tasks/{id} 
- POST   /api/learning-tasks 
- PUT    /api/learning-tasks/{id} 
- DELETE /api/learning-tasks/{id} 

- POST   /api/task-assignments 
- GET    /api/task-assignments 
- GET    /api/task-assignments/{id} 
- PUT    /api/task-assignments/{id}/status 

- POST   /api/submissions 
- GET    /api/submissions 
- GET    /api/submissions/{id} 

- POST   /api/reviews 
- GET    /api/reviews 
- GET    /api/reviews/{id}

## Sample Request JSON 

Sample POST /api/trainees request: 
```javascript
{ 
  "firstName": "john", 
  "lastName": "joe", 
  "email": "john.doe@training.com", 
  "techStack": "HTML, CSS, JavaScript", 
  "status": "Active"
} 
```

Sample PUT /api/trainees/{id} request: 
```javascript
{  
  "status": "InActive"
} 
```

Sample POST   /api/mentors  Request: 
```javascript
{
  "firstName": "mentor name",
  "lastName": "mentor lastname",
  "email": "mentor@example.com",
  "expertise": "string",
  "status": "Active"
}
```

Sample PUT    /api/mentors/{id}  Request: 
```javascript
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "expertise": "string",
  "status": "Active" 
}
```


Sample POST   /api/learning-tasks  Request: 
```javascript
{
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-15T06:23:00.184Z",
  "status": "Draft"
}
```

Sample PUT    /api/learning-tasks/{id}  Request: 
```javascript
{
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-15T06:23:00.184Z",
  "status": "Draft"
}
```

Sample POST   /api/task-assignments  Request: 
```javascript
{
  "traineeId": 0,
  "mentorId": 0,
  "learningTaskId": 0,
  "assignedDate": "2026-06-15T06:23:33.674Z",
  "dueDate": "2026-06-15T06:23:33.674Z",
  "status": "Assigned",
  "remarks": "string"
}
```

Sample PUT    /api/task-assignments/{id}/status  Request: 
```javascript
{
  "status": "Assigned",
}
```

Sample POST   /api/submissions  Request: 
```javascript
{
  "taskAssignmentId": 0,
  "submissionUrl": "string",
  "notes": "string",
  "submissionDate": "2026-06-15T06:24:12.398Z",
  "status": "Submitted"
}
```

Sample POST   /api/reviews  Request: 
```javascript
{
  "submissionId": 0,
  "mentorId": 0,
  "feedback": "string",
  "score": "string",
  "status": "Accepted",
  "reviewedDate": "2026-06-15T06:24:27.235Z"
}
```

## Sample Response JSON
Sample GET /api/health response:
```javascript
{
  "status": "running",
  "application": "Trainee Management App",
  "timestamp": "2026-06-08T11:10:34.0036979+00:00"
}
```

Sample POST /api/auth/login response:
```javascript
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWljaGFlbCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzgxNTE1NDAzLCJpc3MiOiJUcmFpbmVlTWFuYWdlbWVudEFwaSIsImF1ZCI6IlRyYWluZWVNYW5hZ2VtZW50Q2xpZW50In0.fN3A2xkRlHsMobn4elShkrS7CaS0yxe_OEt_5FH84p0",
  "expiresIn": "2026-06-15T09:23:23Z",
  "user": {
    "id": 0,
    "username": "michael",
    "email": "michael@gmail.com",
    "role": "Admin",
    "createdDate": "0001-01-01T00:00:00",
    "updatedDate": "0001-01-01T00:00:00"
  }
}
```


Sample GET /api/trainees response: 
```javascript
[
  {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active",
    "createdDate": "2026-06-08T10:55:05.7288647+00:00",
    "updatedDate": "2026-06-08T10:55:05.7294876+00:00"
  }
]
```

Sample POST /api/trainees response: 
```javascript
{
  "newTrainee": {
    "id": 1,
    "firstName": "john",
    "lastName": "joe",
    "email": "john.doe@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active",
    "createdDate": "2026-06-08T10:55:05.7288647+00:00",
    "updatedDate": "2026-06-08T10:55:05.7294876+00:00"
  }
}
```

Sample GET /api/trainees/{id} response: 
```javascript
{
  "id": 1,
  "firstName": "john",
  "lastName": "joe",
  "email": "john.doe@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Active",
  "createdDate": "2026-06-08T10:55:05.7288647+00:00",
  "updatedDate": "2026-06-08T10:55:05.7294876+00:00"
}
```

Sample PUT /api/trainees/{id} response:
```javascript
{
  "id": 1,
  "firstName": "john",
  "lastName": "joe",
  "email": "john.doe@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Inactive",
  "createdDate": "2026-06-08T10:55:05.7288647+00:00",
  "updatedDate": "2026-06-08T10:57:22.9859447+00:00"
}
```

Sample GET    /api/mentors  Response: 
```javascript
[
  {
    "id": 1,
    "firstName": "mentor",
    "lastName": "string",
    "email": "mentor@gmail.com",
    "expertise": "Web development",
    "status": "Active",
    "createdDate": "2026-06-11T13:04:19.796316",
    "updatedDate": "2026-06-11T13:16:48.646818",
    "taskAssignments": [
      {
        "id": 1,
        "traineeId": 4,
        "mentorId": 1,
        "learningTaskId": 1,
        "assignedDate": "2026-06-12T10:16:43.026",
        "dueDate": "2026-06-12T10:16:43.026",
        "status": "Assigned",
        "remarks": null,
        "submissions": []
      },
      {
        "id": 2,
        "traineeId": 5,
        "mentorId": 1,
        "learningTaskId": 1,
        "assignedDate": "2026-06-12T10:16:43.026",
        "dueDate": "2026-07-12T10:16:43.026",
        "status": "InProgress",
        "remarks": null,
        "submissions": []
      },
      {
        "id": 3,
        "traineeId": 5,
        "mentorId": 1,
        "learningTaskId": 1,
        "assignedDate": "2026-06-12T10:16:43.026",
        "dueDate": "2026-05-12T10:16:43.026",
        "status": "Assigned",
        "remarks": null,
        "submissions": []
      }
    ],
    "reviews": [
      {
        "id": 1,
        "submissionId": 1,
        "mentorId": 1,
        "feedback": "everything is fine",
        "score": "9/10",
        "status": "Accepted",
        "reviewedDate": "2026-06-12T12:42:15.378"
      }
    ]
  },
  {
    "id": 2,
    "firstName": "string",
    "lastName": "string",
    "email": "mentor2@example.com",
    "expertise": "string",
    "status": "Active",
    "createdDate": "2026-06-11T13:17:16.029161",
    "updatedDate": "2026-06-12T08:07:57.476317",
    "taskAssignments": [
      {
        "id": 4,
        "traineeId": 6,
        "mentorId": 2,
        "learningTaskId": 2,
        "assignedDate": "2026-06-12T10:38:31.091",
        "dueDate": "2026-07-12T10:38:31.091",
        "status": "Assigned",
        "remarks": null,
        "submissions": []
      }
    ],
    "reviews": []
  }
]
```

Sample GET    /api/mentors/{id}  Response: 
```javascript
{
  "id": 1,
  "firstName": "mentor",
  "lastName": "string",
  "email": "mentor@gmail.com",
  "expertise": "Web development",
  "status": "Active",
  "createdDate": "2026-06-11T13:04:19.796316",
  "updatedDate": "2026-06-11T13:16:48.646818",
  "taskAssignments": [],
  "reviews": []
}
```

Sample POST   /api/mentors  Response: 
```javascript
{
  "id": 4,
  "firstName": "mentor name",
  "lastName": "mentor lastname",
  "email": "mentor@example.com",
  "expertise": "string",
  "status": "Active",
  "createdDate": "2026-06-15T06:25:46.4344634+00:00",
  "updatedDate": "2026-06-15T06:25:46.4344807+00:00"
}
```

Sample PUT    /api/mentors/{id}  Response: 
```javascript
{
  "id": 4,
  "firstName": "mentor name",
  "lastName": "mentor lastnaem",
  "email": "mentor.name@example.com",
  "expertise": "Asp.net core",
  "status": "Active",
  "createdDate": "2026-06-15T06:25:46.434463",
  "updatedDate": "2026-06-15T06:27:15.1490412+00:00",
  "taskAssignments": [],
  "reviews": []
}
```


Sample GET    /api/learning-tasks  Response: 
```javascript
[
  {
    "id": 1,
    "title": "string",
    "description": "string",
    "expectedTechStack": "string",
    "dueDate": "2026-06-12T07:23:42.199",
    "status": "Closed",
    "createdDate": "2026-06-12T06:11:03.554699",
    "updatedDate": "2026-06-12T08:10:51.633823",
    "taskAssignments": [
      {
        "id": 3,
        "traineeId": 5,
        "mentorId": 1,
        "learningTaskId": 1,
        "assignedDate": "2026-06-12T10:16:43.026",
        "dueDate": "2026-05-12T10:16:43.026",
        "status": "Assigned",
        "remarks": null,
        "submissions": []
      },
      {
        "id": 2,
        "traineeId": 5,
        "mentorId": 1,
        "learningTaskId": 1,
        "assignedDate": "2026-06-12T10:16:43.026",
        "dueDate": "2026-07-12T10:16:43.026",
        "status": "InProgress",
        "remarks": null,
        "submissions": []
      },
      {
        "id": 1,
        "traineeId": 4,
        "mentorId": 1,
        "learningTaskId": 1,
        "assignedDate": "2026-06-12T10:16:43.026",
        "dueDate": "2026-06-12T10:16:43.026",
        "status": "Assigned",
        "remarks": null,
        "submissions": []
      }
    ]
  },
  {
    "id": 2,
    "title": "intermediate",
    "description": "learn intermediate stuff",
    "expectedTechStack": "dotnet",
    "dueDate": "2026-06-12T06:10:41.504",
    "status": "Draft",
    "createdDate": "2026-06-12T06:11:23.192005",
    "updatedDate": "2026-06-12T06:11:23.192006",
    "taskAssignments": [
      {
        "id": 4,
        "traineeId": 6,
        "mentorId": 2,
        "learningTaskId": 2,
        "assignedDate": "2026-06-12T10:38:31.091",
        "dueDate": "2026-07-12T10:38:31.091",
        "status": "Assigned",
        "remarks": null,
        "submissions": []
      }
    ]
  }
]
```

Sample GET    /api/learning-tasks/{id}  Response: 
```javascript
{
  "id": 1,
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-12T07:23:42.199",
  "status": "Closed",
  "createdDate": "2026-06-12T06:11:03.554699",
  "updatedDate": "2026-06-12T08:10:51.633823",
  "taskAssignments": []
}
```

Sample POST   /api/learning-tasks  Response: 
```javascript
{
  "id": 4,
  "title": "string",
  "description": "string",
  "expectedTechStack": "string",
  "dueDate": "2026-06-15T06:23:00.184Z",
  "status": "Draft",
  "createdDate": "2026-06-15T06:28:36.1590737+00:00",
  "updatedDate": "2026-06-15T06:28:36.1590886+00:00"
}
```

Sample PUT    /api/learning-tasks/{id}  Response: 
```javascript
{
  "id": 4,
  "title": "auth",
  "description": "implement auth",
  "expectedTechStack": "asp.net",
  "dueDate": "2026-06-20T06:28:55.685Z",
  "status": "Draft",
  "createdDate": "2026-06-15T06:28:36.159073",
  "updatedDate": "2026-06-15T06:29:54.7039602+00:00",
  "taskAssignments": []
}
```

Sample GET    /api/task-assignments  Response: 
```javascript
[
  {
    "id": 1,
    "traineeId": 4,
    "mentorId": 1,
    "learningTaskId": 1,
    "assignedDate": "2026-06-12T10:16:43.026",
    "dueDate": "2026-06-12T10:16:43.026",
    "status": "Assigned",
    "remarks": null,
    "submissions": [
      {
        "id": 1,
        "taskAssignmentId": 1,
        "submissionUrl": "github.com",
        "notes": "none",
        "submissionDate": "2026-06-12T12:14:19.144",
        "status": "Submitted",
        "reviews": []
      }
    ]
  },
  {
    "id": 2,
    "traineeId": 5,
    "mentorId": 1,
    "learningTaskId": 1,
    "assignedDate": "2026-06-12T10:16:43.026",
    "dueDate": "2026-07-12T10:16:43.026",
    "status": "InProgress",
    "remarks": null,
    "submissions": [
      {
        "id": 2,
        "taskAssignmentId": 2,
        "submissionUrl": "gitlab.com",
        "notes": "none",
        "submissionDate": "2026-03-12T12:14:19.144",
        "status": "Resubmitted",
        "reviews": []
      }
    ]
  },
  {
    "id": 3,
    "traineeId": 5,
    "mentorId": 1,
    "learningTaskId": 1,
    "assignedDate": "2026-06-12T10:16:43.026",
    "dueDate": "2026-05-12T10:16:43.026",
    "status": "Assigned",
    "remarks": null,
    "submissions": []
  },
  {
    "id": 4,
    "traineeId": 6,
    "mentorId": 2,
    "learningTaskId": 2,
    "assignedDate": "2026-06-12T10:38:31.091",
    "dueDate": "2026-07-12T10:38:31.091",
    "status": "Assigned",
    "remarks": null,
    "submissions": []
  }
]
```

Sample GET    /api/task-assignments/{id}  Response: 
```javascript
{
  "id": 1,
  "traineeId": 4,
  "mentorId": 1,
  "learningTaskId": 1,
  "assignedDate": "2026-06-12T10:16:43.026",
  "dueDate": "2026-06-12T10:16:43.026",
  "status": "Assigned",
  "remarks": null,
  "submissions": []
}
```

Sample POST   /api/task-assignments  Response: 
```javascript
{
  "traineeId": 5,
  "mentorId": 1,
  "learningTaskId": 1,
  "assignedDate": "2026-06-15T06:23:33.674Z",
  "dueDate": "2026-06-15T06:23:33.674Z",
  "status": "Assigned",
  "remarks": null
}
```

Sample PUT    /api/task-assignments/{id}/status  Response: 
```javascript
{
  "id": 1,
  "traineeId": 4,
  "mentorId": 1,
  "learningTaskId": 1,
  "assignedDate": "2026-06-12T10:16:43.026",
  "dueDate": "2026-06-12T10:16:43.026",
  "status": "Reviewed",
  "remarks": null,
  "submissions": []
}
```

Sample GET    /api/submissions  Response: 
```javascript
[
  {
    "id": 1,
    "taskAssignmentId": 1,
    "submissionUrl": "github.com",
    "notes": "none",
    "submissionDate": "2026-06-12T12:14:19.144",
    "status": "Submitted",
    "reviews": [
      {
        "id": 1,
        "submissionId": 1,
        "mentorId": 1,
        "feedback": "everything is fine",
        "score": "9/10",
        "status": "Accepted",
        "reviewedDate": "2026-06-12T12:42:15.378"
      }
    ]
  },
  {
    "id": 2,
    "taskAssignmentId": 2,
    "submissionUrl": "gitlab.com",
    "notes": "none",
    "submissionDate": "2026-03-12T12:14:19.144",
    "status": "Resubmitted",
    "reviews": []
  }
]
```

Sample GET    /api/submissions/{id}  Response: 
```javascript
{
  "id": 2,
  "taskAssignmentId": 2,
  "submissionUrl": "gitlab.com",
  "notes": "none",
  "submissionDate": "2026-03-12T12:14:19.144",
  "status": "Resubmitted",
  "reviews": []
}
```

Sample POST   /api/submissions  Response: 
```javascript
{
  "id": 3,
  "taskAssignmentId": 1,
  "submissionUrl": "string",
  "notes": "string",
  "submissionDate": "2026-06-15T06:24:12.398Z",
  "status": "Submitted",
  "taskAssignment": null
}
```

Sample GET    /api/reviews  Response: 
```javascript
[
  {
    "id": 1,
    "submissionId": 1,
    "mentorId": 1,
    "feedback": "everything is fine",
    "score": "9/10",
    "status": "Accepted",
    "reviewedDate": "2026-06-12T12:42:15.378"
  }
]
```

Sample GET    /api/reviews/{id} Response: 
```javascript
{
  "id": 1,
  "submissionId": 1,
  "mentorId": 1,
  "feedback": "everything is fine",
  "score": "9/10",
  "status": "Accepted",
  "reviewedDate": "2026-06-12T12:42:15.378"
}
```

Sample POST   /api/reviews  Response: 
```javascript
{
  "id": 2,
  "submissionId": 2,
  "mentorId": 2,
  "feedback": "string",
  "score": "string",
  "status": "Accepted",
  "reviewedDate": "2026-06-15T06:49:17.725Z",
  "submission": null,
  "mentor": null
}
```

## Known Limitations 
- Scalability

## Security checklist

## Next improvement areas.
- Improve scalability
