
const PROXY_CONFIG = [
  {
    context: [
      "/api",
    ],
    target: "https://localhost:7266",
    secure: false
  },
  {
    context: [
      "/taco-api",
    ],
    "target": "http://localhost:4000",
    "secure": false,
    "pathRewrite": {
      "^/taco-api": "/graphql"
    }
  }
]

module.exports = PROXY_CONFIG;
