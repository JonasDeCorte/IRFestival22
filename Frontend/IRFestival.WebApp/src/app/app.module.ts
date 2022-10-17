import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './layout/header/header.component';
import { FooterComponent } from './layout/footer/footer.component';
import { LayoutModule } from './layout/layout.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {
  MsalInterceptor,
  MsalModule,
  MsalRedirectComponent,
} from '@azure/msal-angular';
import { InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { environment } from 'src/environments/environment';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    LayoutModule,
    MsalModule.forRoot(
      new PublicClientApplication({
        auth: {
          clientId: '6731a943-086d-4621-a96a-33e2f74c2981',
          authority:
            'https://login.microsoftonline.com/c019c988-b7fa-474f-8a52-3af29c0636e4',
          redirectUri: environment.redirectUrl,
        },
        cache: {
          cacheLocation: 'localStorage',
        },
      }),
      null,
      {
        interactionType: InteractionType.Redirect,
        protectedResourceMap: new Map([
          [
            environment.apiBaseUrl + 'pictures/upload',
            ['api://72eeab28-ba78-4f34-ac4f-a891d2f32a8a/Pictures.Upload.All'],
          ],
        ]),
      }
    ),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent, MsalRedirectComponent],
})
export class AppModule {}
