#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let res = reqwest::get("https://raw.githubusercontent.com/astorDev/minilang/refs/heads/main/hello.json")
        .await?
        .json::<serde_json::Value>()
        .await?;

    println!("{}", serde_json::to_string_pretty(&res)?);
    Ok(())
}