FROM mcr.microsoft.com/dotnet/runtime:5.0-windowsservercore-ltsc2019

WORKDIR /app

COPY ./WpfApp1/bin/Release/net8.0-windows/ .

ENTRYPOINT ["dotnet", "WpfApp1.dll"]
