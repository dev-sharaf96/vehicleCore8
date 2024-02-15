import './polyfills';
import { enableProdMode } from '@angular/core';
import 'zone.js/dist/zone';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { QouteModule } from './app/qoute.module';
import { environment } from './environments/environment';

// if (environment.production) {
  enableProdMode();
// }
platformBrowserDynamic().bootstrapModule(QouteModule)
	.catch(err => console.log(err));
