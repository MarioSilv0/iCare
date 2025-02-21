
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
      "/taco",
    ],
    "target": "http://localhost:4000",
    "secure": false,
    "pathRewrite": {
      "^/taco": "/graphql"
    },
    "changeOrigin": true,
    "logLevel": "debug"
  }
]

module.exports = PROXY_CONFIG;
