# Developing Solutions for Microsoft Azure AZ-204 Exam Guide

<a href="https://www.packtpub.com/product/developing-solutions-for-microsoft-azure-az-204-exam-guide/9781803237060?utm_source=github&utm_medium=repository&utm_campaign=9781803237060"><img src="https://static.packt-cdn.com/products/9781803237060/cover/smaller" alt="About the Authors" height="256px" align="right"></a>

This is the code repository for [Developing Solutions for Microsoft Azure AZ-204 Exam Guide](https://www.packtpub.com/product/developing-solutions-for-microsoft-azure-az-204-exam-guide/9781803237060?utm_source=github&utm_medium=repository&utm_campaign=9781803237060), published by Packt.

**Discover the essentials for success when developing and maintaining cloud-based solutions on Azure**

## What is this book about?
With the prevalence of cloud technologies and DevOps ways of working, the industry demands developers who can build cloud solutions and monitor them throughout their lifecycle. Becoming a Microsoft certified Azure developer can differentiate developers from the competition, but with such a plethora of information available, it can be difficult to structure learning in an effective way to obtain certification. 

This book covers the following exciting features:
* Develop Azure compute solutions
* Discover tips and tricks from Azure experts for interactive learning
* Use Cosmos DB storage and blob storage for developing solutions
* Develop secure cloud solutions for Azure
* Use optimization, monitoring, and troubleshooting for Azure solutions
* Develop Azure solutions connected to third-party services

If you feel this book is for you, get your [copy](https://www.amazon.com/dp/1803237066) today!

<a href="https://www.packtpub.com/?utm_source=github&utm_medium=banner&utm_campaign=GitHubBanner"><img src="https://raw.githubusercontent.com/PacktPublishing/GitHub/master/GitHub.png" 
alt="https://www.packtpub.com/" border="5" /></a>

## Instructions and Navigations
All of the code is organized into folders. For example, Chapter02.

A block of code is set as follows:
```
using Microsoft.Identity.Client;
const string _clientId = "<app/client ID>";
const string _tenantId = "<tenant ID>";
var app = PublicClientApplicationBuilder
```
When we wish to draw your attention to a particular part of a code block, the relevant lines or items are set in bold:
<pre>var app = PublicClientApplicationBuilder
    .Create(_clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
    .WithRedirectUri(<b>"http://localhost"</b>)
    .Build();</pre>

Any command-line input or output is written as follows:
<pre><b>$ dotnet new console -n "&lt;app name&gt;"</b></pre>


**Following is what you need for this book:**
This book is for Azure developers looking to improve their Azure development knowledge to pass the AZ-204 exam. This book assumes at least one year of professional development experience with Azure, with the ability to program in at least one language supported by Azure. Existing Azure CLI and PowerShell skills will also be useful.

With the following software and hardware list you can run all code files present in the book (Chapter 1-14).
### Software and Hardware List
| Software required | OS required |
| ------------------------------------ | ----------------------------------- |
| Visual Studio Code | Windows, macOS, or Linux |
| Docker Desktop | Windows, macOS, or Linux |
| PowerShell Core | Windows, macOS, or Linux |
| Azure CLI | Windows, macOS, or Linux |
| .NET 6.0 | Windows, macOS, or Linux |
| Git | Windows, macOS, or Linux |
| Azure Functions Core Tools | Windows, macOS, or Linux |

We also provide a PDF file that has color images of the screenshots/diagrams used in this book. [Click here to download it](https://packt.link/1TGWe).

## Code in Action

Click on the following link to see the Code in Action:
[Developing Solutions for Microsoft Azure AZ-204 Exam Guide playlist](https://bit.ly/3LtUSAp)


### Related products
* Azure for Developers - Second Edition [[Packt]](https://www.packtpub.com/product/azure-for-developers/9781803240091?utm_source=github&utm_medium=repository&utm_campaign=9781803240091) [[Amazon]](https://www.amazon.com/dp/1803240091)

* Infrastructure as Code with Azure Bicep [[Packt]](https://www.packtpub.com/product/infrastructure-as-code-with-azure-bicep/9781801813747?utm_source=github&utm_medium=repository&utm_campaign=9781801813747) [[Amazon]](https://www.amazon.com/dp/1801813744)

## Get to Know the Authors
**Paul Ivey**
is an experienced engineer and architect specializing in Microsoft technologies, both on-premises and in the Azure cloud.
In his four years at Microsoft, Paul has been a secure infrastructure engineer and an app innovation engineer, where he helped hundreds of enterprise customers adopt DevOps practices and develop solutions for Azure.
Paul is now a Microsoft Technical Trainer, providing training for Microsoft customers to help them with preparing to pass Azure exams.
In his spare time, Paul is a keen PC gamer and enjoys traveling abroad to experience foreign sights, cultures, and food (mostly food).
Originally from Devon in the UK, Paul currently lives in Cheltenham in the beautiful Cotswolds area of England.

**Alex Ivanov**
is an experienced cloud engineer with a primary focus on supporting companies in their journeys to adopt Azure services. Alex has worked for Microsoft for eight years as a cloud support engineer and four years as an Azure Technical Trainer.
Alex is an expert in software engineering and digital transformation who has helped many customers to migrate their solutions to Azure. His experience has helped him gain multiple certifications in software development, AI, and data platforms. As a professional trainer, Alex has already educated thousands of clients and helped them to prepare for and pass the Azure certification exams.
In his free time, while not being jumped on by his three kids, he enjoys camping, boating, running, and building RC models.
### Download a free PDF

 <i>If you have already purchased a print or Kindle version of this book, you can get a DRM-free PDF version at no cost.<br>Simply click on the link to claim your free PDF.</i>
<p align="center"> <a href="https://packt.link/free-ebook/9781803237060">https://packt.link/free-ebook/9781803237060 </a> </p>