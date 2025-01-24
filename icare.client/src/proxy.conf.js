
const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    target: "https://localhost:7266",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
