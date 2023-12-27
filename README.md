# Azure AI and Semantic Kernel Workshop
Workshop content for CodeMash 2024

## Prerequisites

- [Install the .NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) 
	- You must be on v17.8.0 or higher to use .NET 8
- Clone the [git repository](https://github.com/IntegerMan/MattEland.AI.Semantic.Workshop) locally
	- Be sure to pull the latest changes before the workshop
- Create an [Azure Account](https://azure.microsoft.com/en-us/free/) and an **Azure AI Services** resource, ideally in the **East US** region
	- You will need the key, endpoint, and region during the first part of the workshop
	- Use of Azure AI Services will incur [costs on a per-use basis](https://azure.microsoft.com/en-us/pricing/calculator/?ef_id=_k_7189bd4eed8e1189cb09d8c29758101f_k_&OCID=AIDcmm5edswduu_SEM__k_7189bd4eed8e1189cb09d8c29758101f_k_&msclkid=7189bd4eed8e1189cb09d8c29758101f)
- If able due to waitlists / preview resources, create an **Azure OpenAI** resource, ideally in the **East US** region
	- We'll walk through configuring this resource in the workshop for those able to create the preview resource
	- Use of Azure OpenAI will incur [costs on a per-use basis](https://azure.microsoft.com/en-us/pricing/calculator/?ef_id=_k_7189bd4eed8e1189cb09d8c29758101f_k_&OCID=AIDcmm5edswduu_SEM__k_7189bd4eed8e1189cb09d8c29758101f_k_&msclkid=7189bd4eed8e1189cb09d8c29758101f)
- [Create an OpenAI API account](https://platform.openai.com/)
	- Set up a billing method and add a credit balance. I recommend at least $10 for the workshop to ensure you don't run out of credits
	- Note: there's a short delay in setting up a credit balance and it being available for use, so do this before the workshop.
	- Note: If you have an Azure OpenAI resource, you can use that instead of the OpenAI API account for most things, aside from DALL-E image generation

## Setup

TODO: Detail the setup steps a user must take.

## Workshop Structure

### Part 1: Azure AI Services

Azure AI Language to detect entities, sentiment, and PII in a string
Azure AI Vision API v4 to analyze images
Azure Speech to generate and interpret speech

### Part 2: Azure OpenAI

2 minute brief on LLMs
Using Azure OpenAI for Text and Chat Completion
5 minute course in Prompt Engineering
Image generation with DALL-E
5 minute conceptual overview of Text Embeddings

### Part 3: Semantic Kernel Basics

Limitations of LLMs and the need for AI Orchestration
High-level overview of SK

Creating a Kernel and a Semantic Function
Chaining together two semantic functions

### Part 4: Semantic Kernel Plugins & Planners

Creating a Planner
Creating a Plugin
Comparison of SK Planners

More complex SK use-case

SK at Scale: the need for modularity, assistants & scalability, and testing & PromptFlow
