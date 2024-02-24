---

layout: post
title: Get started with the Azure CLI
date: 2024-04-22 16:00:00 0000
categories: cloud

---

# How to use the Azure CLI

## Introduction

The Azure CLI is a command-line tool that you can use to manage your Azure resources. We'll be learning how to use the Azure CLI to create and manage resources in Azure in this tutorial. Let's get started. Watch the full video on [YouTube](https://youtu.be/y9LcRGU6xX4)

## Azure CLI OS Compatibility

The Azure CLI is available for Windows, Mac, and Linux. You can also use the Azure Cloud Shell, which is a browser-based shell that you can use to manage your Azure resources. For our demos, I will be using the Azure CLI on my Windows Machine, but the same principles apply to the other operating systems whether you're on Mac, Linux, Windows or the Azure Cloud Shell.

## Install the Azure CLI

To install the Azure CLI, I'm not going to make you cram a website, just go on your search engine of choice and search for Install Azure CLI. The first link that looks Microsoft-y is the right one to use. Follow through the instructions to install and you're ready to go.

## Login to the Azure CLI

Azure CLI can be used with any CLI installed on your computer - PowerShell, Bash and the rest of the bunch. I will be using PowerShell in this video. PowerShell is a cross-platform task automation and configuration management framework, consisting of a command-line shell and scripting language. PowerShell is built on top of the .NET Common Language Runtime (CLR) and the .NET Framework and accepts and returns .NET Framework objects. PowerShell is a task-based command-line shell and scripting language; it is designed specifically for system administrators and power users, to rapidly automate the administration of multiple operating systems (Linux, macOS, Unix, and Windows) and the processes related to the applications that run on those operating systems.

To login to the Azure CLI, open your PowerShell and type in the following command:

```powershell
az login
```

This will open a browser window and ask you to log in to your Azure account. Once you've logged in, you can close the browser window and go back to your PowerShell. You should see a message on the browser that says you've logged in successfully.

You should then see a list of your subscriptions. If you have more than one subscription, you can select the subscription you want to use by typing in the following command:

```powershell
az account set --subscription <subscription-id>
```

## Create a Resource Group with the Azure CLI

The easiest task you can perform with the Azure CLI is to create a resource group, so let's do just that. To create a resource group, type in the following command:

```powershell
$resourceGroupName="ccd-tut-rg"
$resourceGroupLocation="uksouth"
az group create --name $resourceGroupName --location $resourceGroupLocation
```

The `location` parameter is the Azure region where you want to create the resource group. You can get a list of all the Azure regions by typing in the following command:

```powershell
az account list-locations
```

and the `name` parameter is the name of the resource group you want to create. You can name your resource group anything you want, but it's best to use a name that describes the purpose of the resource group. For example, if you're creating a resource group for your web app, you can name it `webapp-rg`.

Parameters in Azure CLI start with the `--` prefix and are followed by the parameter name. The parameter name is followed by the parameter value. For example, in the command above, the `--name` parameter is followed by the name of the resource group you want to create.

Parameters can also be specified by using shorthand keywords. For example, `--location` has a shorthand of `-l` and `--name` has a shorthand of `-n`. So the command we used to create the resource group earlier can also be done using the following command.

```powershell
$resourceGroupName="ccd-tut-rg"
$resourceGroupLocation="uksouth"
az group create -n $resourceGroupName -l $resourceGroupLocation
```

You will notice I used `resourceGroupName` and `$resourceGroupLocation` in the commands. There are called variables in PowerShell and they come in handy for reusability of values.

Now that we have a resource group, let's create a web app, let's look at some customizations we can make to our Azure CLI instance.

## Customize the Azure CLI

If you noticed, the output we got from our resource group creation command was JSON. This is the default output type. We can customize this to be YAML or TSV. Let's try that.

To make your default output YAML, all you have to do is run this:

```powershell
az config set core.output=yaml
```

Now let's try go get the details of that resource group we created earlier.

```powershell
$resourceGroupName="ccd-tut-rg"
$resourceGroupLocation="uksouth"
az group show --name $resourceGroupName --location $resourceGroupLocation
```

To customize our output on the fly, we can do this:

```powershell
az group show --name $resourceGroupName --location $resourceGroupLocation --output tsv
```

You'll see this has overridden our default configuration. And that's quite fun.

Here are some more customizations you can make to the Azure CLI.

| Customization | Command |
| --- | --- |
| Set Default Output | `az config set core.output=<format>` |
| Set Default Location | `az config set defaults.location=<location>` |
| Set Default Resource Group | `az config set defaults.group=<resource-group>` |
| Set Default Subscription | `az config set defaults.subscription=<subscription-id>` |
| Enable Yes Prompt | `az config set core.no_prompt=True` |
| Disable Telemetry | `az config set core.collect_telemetry=False` |

Note that the `az config set` command is used to set the default configuration for the Azure CLI. You can also use the `az config get` command to get the current configuration of the Azure CLI. You should also always replace the entire text inside the `< >` with the value you want to use.

There is no better way to learn how to use something than to use it. So let's use the Azure CLI more and more.

## Create and Deploy a Web App to Azure

As an example of a practical use case for Azure CLI, let's create a web app and deploy it to Azure. To do this, we'll need to do the following:

- Create a Resource Group
- Create a Web App
- Write a simple web app
- Deploy the web app to Azure
- Clean up

### Create a Resource Group

We already know how to create a resource group, so let's do that.

```powershell
$resourceGroupName="ccd-webapp-tut-rg"
$resourceGroupLocation="uksouth"
az group create --name $resourceGroupName --location $resourceGroupLocation
```

Once the resource group is created, we can move on to the next step.

### Create a Web App

To create a web app, we'll need to use the `az webapp create` command. This command has a lot of parameters, but we'll only be using a few of them.

An app service lives in an app service plan. An app service plan is a set of compute resources for a web app to run. You can have multiple web apps running in a single app service plan. The app service plan is what determines the pricing tier of your web app. The pricing tier determines the amount of resources available to your web app. The higher the pricing tier, the more resources available to your web app. The pricing tier also determines the cost of your web app. The higher the pricing tier, the more expensive your web app will be.

Let's create an app service plan for our web app.

```powershell
$appServicePlanName="ccd-webapp-tut-asp"
$appServicePlanLocation="uksouth"
$appServicePlanSku="F1"
$resourceGroupName="ccd-webapp-tut-rg"
az appservice plan create --name $appServicePlanName --location $appServicePlanLocation --sku $appServicePlanSku --resource-group $resourceGroupName
```

The `--sku` parameter is the pricing tier of the app service plan. The `F1` pricing tier is the free tier.

Now that we have an app service plan, we can create our web app.

```powershell
$webAppName="ccd-webapp-tut"
$appServicePlanName="ccd-webapp-tut-asp"
$resourceGroupName="ccd-webapp-tut-rg"
az webapp create --name $webAppName --plan $appServicePlanName --resource-group $resourceGroupName
```

Now that we have a web app, let's see what we have created in the Azure Portal. We'll to the Azure Portal and search for the resource group we created earlier. We should see the resource group and the resources we created in it.

### Write a Simple Web App

Let's get a simple web app up into our web app. We will be using a bootstrapped ASP.NET Core MVC web app. I simply create this by running the following command:

```powershell
dotnet new mvc -o CCD.WebApp
```

Note, I already have the .NET Core SDK installed on my machine. If you don't have the .NET Core SDK installed on your machine, you can download it from [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) if you want to use a .NET App. If you want to use a different app, you can use that instead.

### Deploy the Web App to Azure

Now that we have a web app, let's deploy it to Azure. First, let use change our directory to the web app directory.

```powershell
cd CCD.WebApp
```

The next action we need to take is run the `az webapp up` command. This command will deploy our web app to Azure.

```powershell
$webAppName="ccd-webapp-tut"
az webapp up --name $webAppName -resource-group $resourceGroupName
```

After some seconds, you should see a message that says your web app has been deployed successfully. Let's go to the Azure Portal and see what we have. And there it is, our web app has been deployed successfully.

We can navigate to the URL of our web app and see what we have. And there it is, our web app is up and running.

Any subsequent changes we make to our web app can be deployed to Azure by running the `az webapp up` command again.

### Clean Up

Since this is a demo, we don't want to leave our resources running in Azure. So let's clean up. To clean up, we'll need to delete the resource group we created earlier.

```powershell
$resourceGroupName="ccd-webapp-tut-rg"
az group delete --name $resourceGroupName
```

And we are done and our resources have been deleted.

## Wrap Up

What I've shown you in this tutorial is just a tip of the iceberg. There are so many things you can do with the Azure CLI. I encourage you to explore the Azure CLI and see what you can do with it. You can find more information about the Azure CLI at [https://docs.microsoft.com/cli/azure/](https://docs.microsoft.com/cli/azure/).

If you have any questions, feel free to ask by [filling this form](https://forms.gle/7a8YwiSH6GJQBT8j8) and if you have general inquiries about Code, Cloud and DevOps, [fill this form](https://forms.gle/nedwikkuZJRrSFBo8). I'll see you again soon.
