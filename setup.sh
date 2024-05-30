#!/usr/bin/bash

dotnet new blazor --interactivity Server -o . --use-program-main

dotnet new gitignore

dotnet user-secrets init

dotnet add package Microsoft.Extensions.Configuration

dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.17


