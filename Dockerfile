# 1. AŞAMA: İNŞAAT (Kodu derle, "Build")
# Microsoft'un resmi .NET 9.0 "SDK" (Geliştirme Kiti) imajını kullan
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# ÖNCE SADECE PROJE DOSYASINI KOPYALA
COPY SimpleApi.csproj .

# Bağımlılıkları restore et
RUN dotnet restore "SimpleApi.csproj"

# ŞİMDİ KODUN GERİ KALANINI KOPYALA
COPY . .

# Projeyi derle
RUN dotnet build "SimpleApi.csproj" -c Release -o /app/build


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "SimpleApi.dll"]