# Contributing to PSProxmox

Thank you for your interest in contributing to PSProxmox! This document provides guidelines and instructions for contributing.

## Code of Conduct

By participating in this project, you agree to abide by the [Code of Conduct](CODE_OF_CONDUCT.md).

## How Can I Contribute?

### Reporting Bugs

This section guides you through submitting a bug report. Following these guidelines helps maintainers understand your report, reproduce the behavior, and find related reports.

- Use the bug report template when creating an issue
- Include detailed steps to reproduce the problem
- Describe the behavior you observed and what you expected to see
- Include PowerShell and Proxmox VE version information

### Suggesting Enhancements

This section guides you through submitting an enhancement suggestion, including completely new features and minor improvements to existing functionality.

- Use the feature request template when creating an issue
- Provide a clear and detailed explanation of the feature you want
- Explain why this enhancement would be useful to most PSProxmox users

### Pull Requests

- Fill in the required template
- Follow the C# and PowerShell style guides
- Include tests for new features or bug fixes
- Update documentation as needed
- End all files with a newline

## Development Workflow

1. Fork the repository
2. Create a new branch for your changes
3. Make your changes
4. Add or update tests as needed
5. Run tests to ensure they pass
6. Update documentation as needed
7. Submit a pull request

## Style Guides

### Git Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests liberally after the first line

### C# Style Guide

- Follow the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Use 4 spaces for indentation
- Use camelCase for private fields and parameters
- Use PascalCase for public/protected properties and methods
- Add XML documentation comments for all public members

### PowerShell Style Guide

- Follow the [PowerShell Best Practices and Style Guide](https://github.com/PoshCode/PowerShellPracticeAndStyle)
- Use proper verb-noun naming for cmdlets
- Include comment-based help for all cmdlets
- Use pipeline input where appropriate

## Testing

- Write tests for all new features and bug fixes
- Run tests before submitting a pull request
- Ensure all tests pass

## Documentation

- Update the README.md with details of changes to the interface
- Update the CHANGELOG.md with details of changes
- Update XML documentation comments for all public members
- Update comment-based help for all cmdlets
