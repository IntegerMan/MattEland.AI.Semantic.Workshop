# Workshop Outline

## Setup
7:40 - 8:00

Ensuring AV works
Testing tech from WiFi
Final discussions
Early setup for early arrivers

## Intro
8:00 AM - 8:15 AM

This is going to be a minimal section to help people orient their expectations, know how to get help, and know what we're covering.
We also want to make sure people know as soon as possible what they need to start installing if they've not installed anything yet.

### High Level Workshop Overview
Talking about the technologies at a high level

### What we assumed you did beforehand
Talk about installing VS 2022 v17.8+, .NET 8, and setting up an Azure AI Services resource and OpenAI key
For those who haven't done this, prioritize .NET 8, VS 2022, and Azure AI Services. We'll have a break before we do the OpenAI work.
Also promise we'll walk through the setup for each part in brief for anyone stuck (applies to Azure and OpenAI only)

### Matt and Sam
Here we introduce ourselves briefly, establishing that it's fine to interrupt, establish how to get help, etc. This also gives some filler time for people to trigger setup actions as needed.

### What we're covering
Talk about the 4 part structure of the workshop and what's covered in each part.
Talk about a largely guided exploration aspect of things with pre-existing code you can interact with, tweak, and change. Light coding at most in these areas.
Also talk about the 3 major labs and the things we'll build as part of each one.

## Part 1: Azure AI Services

### Azure AI Services Setup
8:15 - 8:20

Walk through using the Azure portal to create an Azure AI Services resource, talk about what this is as an umbrella service, and show how to get a key, the endpoint, and the region.
Also talk through the pricing structure on Azure AI services

### Guided Exploration: Azure AI Services
8:20 - 8:50

We start by looking at the `appsettings.json` file and showing where to put those keys, talk about the sensitivity of keys, and then run through examples

#### Analyze Text
We walk through the process of authenticating with Azure and making a request to Azure AI Language.

We walk through the types of result information that comes back:
- Abstractive summarization
- Extractive summarization
- Sentiment analysis
- Key Phrase extraction
- Entity recognition
- Linked Entity Recognition
- PII recognition
- Healthcare entity recognition

We talk about these capabilities at a high level and look at the code for responding to each one of these things.

Code here will not be needed, OR could be done just at the authentication and initiating the request level (though this would probably add 10 minutes which I'd like elsewhere). This will be interactive because people will be able to enter their own text, including copying and pasting text from other sources.

#### Analyze Images
We cover the v4 image analysis APIs by showing the code to authenticate and analyze an image on disk or online.

We cover the concepts and result code for:

- Captions
- Tags
- Object detection
- Person detection
- Dense captioning
- OCR / Text
- Crop suggestions

We could do code-along for making the request, but that'd probably add +5 minutes to the schedule. Instead, we let people provide their own images and analyze them.

#### Remove Background

Here we show the background removal and foreground matting aspects of the v4 image API and show the difference between the two by changing the enum value.
This example can be cut for time in an emergency.

#### Text to Speech

We focus here on voices and the process of speaking the result aloud. This could be an AV challenge, but it's a brief moment and people should be able to replicate it locally.

Giving people an opportunity to synthesize their own speech should be interesting for folks.

#### Speech to Text

This final bit covers speech recognition with RecognizeOnceAsync and should be very brief. No coding required, but people will be able to practice it in action.

### LAB 1: Listen, Summarize, and Speak
8:50 - 9:15

This lab involves coding the things covered in part 1 without error handling. Specifically:

1. Authenticating with Azure AI Speech
2. Recognizing Text as a string
3. Authenticating with Azure AI Language
4. Analyzing Text with abstractive summarization (or other choices - attendee preference)
5. Building a string from the result
6. Speaking that string aloud with a speech synthesizer

I have hints in the file plus a solution provided.

At the conclusion I'll walk people briefly through the solution file if time allows.

## Part 2: Generative AI
Talk about the basics of Generative AI, large language models, and OpenAI vs Azure OpenAI

### OpenAI Setup
Walk through setting up an API key in OpenAI and adding a balance, then stressing the importance of doing this NOW because it takes time.

Dismiss for break and encouraging folks who haven't set up OpenAI to do so over break and to reach out for help if they need it.

### === BREAK 1 ======================================================
9:30 - 9:45

### Guided Exploration: OpenAI
9:45 - 10:15

Here we repeat the exploration phase from earlier on Azure AI but focus instead on OpenAI

#### Text Completions
We walk through the idea of talking to a model and the various parameters that are involved, plus the results. This also stresses the idea of tokens and the idea of text completions. I'll also hint that text completion can be more than just conversational in nature and say how we'll see a few examples of this later.

We also discuss zero-shot, one-shot, and few-shot inferences as concepts.

Encourage people to try tweaking properties such as temperature and max tokens or the prompts.

#### Chat Completions
Talk about chat completion vs text completion and say how we need a different model for this since not all models support all capabilities.

Talk about ChatGPT in general here and let people play with it with their own context.

#### Image Generation
Talk about image generation models and DALL-E 3, then show how DALL-E image generation works. Encourage people to try with their own prompts.

#### Understanding and Generating Embeddings
As time allows, we'll discuss what embeddings are at a VERY high level, then talk about using embedding models to generate embeddings for two text passages to compare their semantic similarity.
This example can be cut for time in an emergency.

#### Searching Text Embeddings
Here we search existing embeddings from some of the CodeMash blog posts to see how semantic search works.
This example can be cut for time in an emergency.

### LAB 2: Image Analysis -> Image Generation
10:15 - 10:35

Here we'll have the attendees complete a lab involving using Azure AI to caption an image, then feed that caption into OpenAI DALL-E to generate the AI's interpretation of what it "sees" in the image

Again, guided hints are present in the file as well as a solution file. I'll also walk through the solution.

## Part 3: Intro to AI Orchestration and Semantic Kernel

Here I'll talk about AI Orchestration and Retrieval Augmentation Generation (RAG) as concepts and why LLMs need their support. We'll then introduce Semantic Kernel as *a* solution to these needs.

### Guided Exploration: Semantic Kernel
10:40 - 11:00

#### Chat with the Kernel
We'll walk through the basics of setting up a kernel and chatting with it. This will be roughly the equivalent of working directly with OpenAI with extra steps at this point.

#### Chatting with Prompt Templates
Here we show how the prompt template can be customized to give a different chat experience.
This example can be cut for time in an emergency.

#### Classification Example
Here we fulfil the earlier promise of showing how a text generation model can be used for something beyond chat. In this case classifying things into known categories from few-shot inference examples.
This example can be cut for time in an emergency.

#### Chaining together Semantic Functions
Finally, we'll talk about chaining together two different semantic functions to show how the output of function A can be an input for function B.

This sets us up well to talk about the need for plugins and plans, which we'll tease before the final break.

### === BREAK 2 ======================================================
11:00 - 11:10

## Part 4: Semantic Kernel Plugins and Planners

### Guided Exploration: Plugins and Planners
11:10 - 11:25

No new setup or concepts are needed here, we're taking things up to the next level in the final hour.

#### Using Plugins
We start with plugins and showing how you can give SK additional context through .NET methods via plugins. The simple example we'll start with will be a time and date provider.
We'll also show how you can have SK call these plugins "automagically"

#### Observability and Kernel Events
The prior example begs for more observability so we talk about kernel events here and the additional information and control you can get over the kernel by subscribing to these events.

#### Handlebars Planner
We *may* cover the Handlebars planner, but I'll likely mention it and skip over it because **it does not work in my experimentation with it!**

#### Function-Calling Stepwise Planner
We'll talk about the experimental function-calling stepwise planner and how it iteratively calls to the LLM to see what the next step it should do is.

#### Excluding functions from plans
We talk about scalability and Microsoft's current recommendation to manually exclude functions from plans.

### LAB 3: Build your own plugins
11:25 - 11:50

The final workshop will be more open than the other two and an encouragement to play around with SK, write your own small plugin, and reach out for help or to show what cool things they built.

## Final Discussions: Scalability and where to go from here
11:50 - 12:00

We'll end with final discussions on where SK is going from where it is right now, what you need to think about as you experiment with it or build larger projects with it / use it in the real world, and where you can go for more information, help, and examples.
If I have time, I'll also modernize my bat-computer example to use the latest SK version and point that out as an example, but I think I likely won't get the time for that by the start of CodeMash.
