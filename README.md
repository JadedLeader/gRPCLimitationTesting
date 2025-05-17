 
# gRPC stress testing tool

## Project Description:
<br> This tool is used for testing gRPC high workloads in conjunction with network constrained environments to calculate the delay between when the client sends the request, and when the server responsds to that request. This is done by using both unary and streaming requests and response types, alongside delving into multiple clients on a single channel, single clients on a single channel and other combinations.

# Disclaimer
<br> This tool is fully developed using gRPC, while I am aware that in a real world scenario where this website is going to be used by the masses it's often better to use REST instead of gRPC (for various reasons) and solely use gRPC for service to service communication implementations as in microservices, however, as this was done as a pet project with the sole purpose being to support my dissertation at university, this wasn't really on my mind and I was more intrigued with using gRPC than sticking to a standard.

## Usage: 
- **Investigating how the different variations of communication can handle varying loads of data**
- **Investigating how port exhaustion can effect gRPC, looking atephemeral ports**
- **Investigating how network constrained environments can effect the overall performance of gRPC communication flows using clumsy**
- **Investigating how channels work, focusing on how clients interact with channels and their various flows**

## Technologies: 
- C#
- gRPC
- EFcore
- Serilog
- JWT
- SSMS
- Worker services
- Blazor server app
- MudBlazor
- Blazored local storage

## Installation and build guide

Follow these steps to get **only** the `gRPCStressTestingService` and `gRPCToolFrontEnd` projects running locally.

### 1. Prerequisites
- [.NET 8.0 SDK (or later)](https://dotnet.microsoft.com/download)  
- [Git](https://git-scm.com/downloads)  
- A local SQL Server instance (SSMS)  
- (Optional) JetBrains Rider or Visual Studio 2022+

### 2. Clone the Repository

git clone https://github.com/JadedLeader/gRPCLimitationTesting.git

### 3. Configure Database Connection (User Secrets)

Each project needs a connection string under `ConnectionStrings:DbConnection`. This is stored in user secrets to not get pushed to source control

Example: 
"ConnectionStrings:DbConnection" \
  "Server=(localdb)\\mssqllocaldb;Database=GrpcStressTestDb;Trusted_Connection=True;"

###4. Run both the gRPCStressTestingService and the gRPCToolFrontEnd 

As long as everything is setup correctly, everything should work accordingly




