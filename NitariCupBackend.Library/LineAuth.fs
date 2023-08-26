namespace NitariCupBackend.Library

open System
open System.Text.Json
open System.Net.Http
open System.Net.Http.Headers
open NitariCupBackend.Library.LineAuthModel

module LineAuth =
    
    let private LineProfileUrl = "https://api.line.me/v2/profile"

    let GetProfile(accesToken: string) =
        task {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Authorization <- new AuthenticationHeaderValue("Bearer", accesToken)

            let! response = client.GetAsync(LineProfileUrl) |> Async.AwaitTask

            if response.StatusCode <> System.Net.HttpStatusCode.OK then
                let profile = { displayName = "Unknown"; userId = "Unknown"; pictureUrl = "Unknown"; statusMessage = "Unknown" }
                return profile
            else
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                let profile = content |> JsonSerializer.Deserialize<LineAuthProfile>

                return profile
        }

