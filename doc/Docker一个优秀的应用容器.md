# 1 什么是Docker？
Docker是一个用于开发，发布和运行应用程序的开放平台。Docker使您能够将应用程序与基础架构分开，从而可以快速交付软件。借助Docker，您可以以与管理应用程序相同的方式来管理基础架构。通过利用Docker的方法来快速交付，测试和部署代码，您可以大大减少编写代码和在生产环境中运行代码之间的延迟。

# 2 为什么需要Docker？
## 2.1 解决环境配置麻烦
一般我们写程序，会有开发环境，测试环境，生产环境，很多bug都是环境问题。

过去部署的思路，在开发环境打包，想跑到Window环境下运行。我们得先在Window下载好.NET Framework，IIS等，配置好对应的环境变量，将包丢到iis的webapps文件夹下，才能跑起来。

现在Docker的思路，可以将我们的想要的环境构建成一个镜像，然后我们可以推送到网上去。想要用这个环境的时候，在网上拉取一份就好了。

## 2.2 解决应用之间隔离
大家一定遇到过在多个应用部署在同一台服务器上，有一个应用出现了问题，导致CPU占100%，其他应用也都受到影响。还有就是比如有些应用用NET技术,有些应用用Php技术,这些不同应用各种的依赖软件都安装在同一个服务器上，可能就会造成各种冲突/无法兼容。

## 2.3 Docker与虚拟机
![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/20210115134656.png)

对于虚拟机，主机服务器从下至上有三个基础层：基础架构，主机操作系统和虚拟机监控程序，最重要的是每个虚拟机都有自己的操作系统和所有必要的库。对于Docker，主机服务器仅具有基础结构和操作系统，最重要的是容器引擎，该容器引擎使容器保持隔离状态，但共享基本的OS服务。

因为容器需要的资源要少得多（例如，它们不需要完整的操作系统），所以它们易于部署并且启动迅速。这样可以提高密度，这意味着可以在同一硬件单元上运行更多服务，从而降低成本。

作为在同一内核上运行的副作用，与VM相比，您获得的隔离更少。

# 3 如何使用Docker？
**首先**，安装Docker ，参考官方文档。

* [Mac](https://docs.docker.com/docker-for-mac/install/)
* [Windows](https://docs.docker.com/docker-for-windows/install/)
* [Ubuntu](https://docs.docker.com/install/linux/docker-ce/ubuntu/)
* [Debian](https://docs.docker.com/install/linux/docker-ce/debian/)
* [CentOS](https://docs.docker.com/install/linux/docker-ce/centos/)
* [Fedora](https://docs.docker.com/install/linux/docker-ce/fedora/)

**然后**，创建Docker.WebApi01项目

确保已选择“启用 Docker 支持”复选框，选择所需的容器类型（Windows 或 Linux）。

![image](https://gitee.com/zcqiand/self-media/raw/master/assets/img/210115/20210115162204.png)

**接着**，我们打开Dockerfile文件， 请参阅[Dockerfile引用](https://docs.docker.com/engine/reference/builder/)，了解其中的命令：
```
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Docker.WebApi01/Docker.WebApi01.csproj", "Docker.WebApi01/"]
RUN dotnet restore "Docker.WebApi01/Docker.WebApi01.csproj"
COPY . .
WORKDIR "/src/Docker.WebApi01"
RUN dotnet build "Docker.WebApi01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Docker.WebApi01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.WebApi01.dll"]
```

上面代码含义说明：

* FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base：指定基础镜像文件mcr.microsoft.com/dotnet/aspnet，标记为5.0-buster-slim，别名为base。
* WORKDIR /app：设置工作目录为/app
* EXPOSE 80：将容器 80 端口暴露出来， 允许外部连接这个端口。
* COPY ["Docker.WebApi01/Docker.WebApi01.csproj", "Docker.WebApi01/"]：将 Docker.WebApi01/Docker.WebApi01.csproj 复制到 Docker.WebApi01/目录下。
* RUN dotnet restore "Docker.WebApi01/Docker.WebApi01.csproj"：恢复 Docker.WebApi01.csproj 项目的依赖项和工具。
* RUN dotnet build "Docker.WebApi01.csproj" -c Release -o /app/build：生成 Docker.WebApi01.csproj 项目及其所有依赖项。
* RUN dotnet publish "Docker.WebApi01.csproj" -c Release -o /app/publish：将应用程序及其依赖项发布到文件夹以部署到托管系统。

**接着**，调试

在工具栏的调试下拉列表中选择“Docker”，然后开始调试应用。 你可能会看到提示信任证书的消息；选择信任证书以继续。

“输出” 窗口中的“容器工具” 选项显示正在进行的操作。 第一次时，可能需要一些时间来下载基本映像，但在后续运行时速度要快得多。

**最后**，构建及运行

```
docker build -t dockerwebapi01 -f ./Docker.WebApi01/Dockerfile .
docker run --rm -it -p 49181:443 dockerwebapi01
```

注意这里有一个坑直接在Dockerfile目录下执行会报以下错误：
```
=> ERROR [build 3/7] COPY [Docker.WebApi01/Docker.WebApi01.csproj, Docker.WebApi01/]
```
遇到这个问题有两个解决方案，其一是把Dockerfile文件放到sln同一个目录下，然后执行docker build，另一个是本文采用的方法，在sln目录下执行,需要指定Dockerfile路径

我们在浏览器上输入： https://localhost:49181/WeatherForecast 看看效果