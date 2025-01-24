import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function bootstrap() {
  platformBrowserDynamic().bootstrapModule(AppModule);
}

bootstrap().catch((err) => console.error(err));
