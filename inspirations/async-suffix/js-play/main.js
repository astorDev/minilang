const res = await fetch("https://raw.githubusercontent.com/astorDev/minilang/refs/heads/main/hello.json");
console.log(await res.json());