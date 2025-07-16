# LLamaSharp Models Directory

This directory contains the AI models used by LLamaSharp for video summarization.

## Required Model

Download a GGUF format model file and place it here with the name `llama-2-7b-chat.gguf`.

## Recommended Models

1. **Llama-2-7B-Chat-GGUF** (Recommended for most users)
   - Size: ~4GB
   - Download from: https://huggingface.co/TheBloke/Llama-2-7B-Chat-GGUF
   - File: `llama-2-7b-chat.Q4_K_M.gguf`

2. **Llama-2-7B-Chat-GGUF** (Smaller, faster)
   - Size: ~2GB
   - Download from: https://huggingface.co/TheBloke/Llama-2-7B-Chat-GGUF
   - File: `llama-2-7b-chat.Q2_K.gguf`

## Setup Instructions

1. Download your preferred model file
2. Rename it to `llama-2-7b-chat.gguf`
3. Place it in this directory
4. Restart the application

## Model Configuration

The model is configured with:
- Context Size: 2048 tokens
- GPU Layers: 0 (CPU only, increase for GPU acceleration)
- Temperature: 0.7 (for balanced creativity)
- Max Tokens: 200 (for concise summaries)

## Fallback Behavior

If no model is found, the application will use a simple text-based summarization as a fallback. 