# 🔍 Small Language Model Samples

This is a sample project that demonstrates how to use **Small Language Models** (SLMs) with a **Retrieval-Augmented Generation (RAG)** pipeline using [LM Studio](https://lmstudio.ai).

You can run everything locally with full privacy and no API keys required.

---

## 🧰 Requirements

- [LM Studio](https://lmstudio.ai) (macOS, Windows, or Linux)
- [.NET SDK 9.0+](https://dotnet.microsoft.com/en-us/download)
- Git

---

## 🚀 Quick Start

### 1. Install LM Studio

Go to [https://lmstudio.ai](https://lmstudio.ai) and download the app for your platform:

- macOS (.dmg)
- Windows (.exe)
- Linux (.AppImage)

> After installation, you can manage models, run the OpenAI-compatible API server, and test prompts locally.

---

### 2. Download a Model via CLI

LM Studio provides a built-in CLI. Open a terminal and run:

```bash
lmstudio get
```

You can browse for Models in the LM Studio interface.

---

### 3. Start the Local Server

To start the OpenAI-compatible API server using LM Studio, run:

```bash
lmstudio server start 
```

This launches an OpenAI-like API at:

```
http://localhost:1234/v1/chat/completions
```

No API key is required.

---

## 📚 Resources

- [LM Studio GitHub](https://github.com/lmstudio-ai/lmstudio)
- [GGUF format](https://github.com/ggerganov/llama.cpp/blob/master/docs/gguf.md)

---

Let me know if you're integrating it with AnythingLLM, a Blazor UI, or a C# backend — I can tailor this README even more specifically.
