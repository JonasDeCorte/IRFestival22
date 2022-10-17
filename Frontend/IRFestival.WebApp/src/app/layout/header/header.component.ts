import { Component, OnInit } from '@angular/core';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';
import { filter, Subject, takeUntil } from 'rxjs';
import { AppSettingsApiService } from 'src/app/api-services/appsettings-api.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {
  festivalName = environment.festivalName;
  authService: MsalService;
  broadcastService: MsalBroadcastService;
  loginDisplay: boolean = false;
  private readonly _destroying$ = new Subject<void>();

  constructor(
    private appSettingsApiService: AppSettingsApiService,
    authService: MsalService,
    broadcastService: MsalBroadcastService
  ) {
    this.authService = authService;
    this.broadcastService = broadcastService;
  }

  ngOnInit(): void {
    this.broadcastService.inProgress$
      // .pipe(
      //   filter(
      //     (status: InteractionStatus) => status === InteractionStatus.None
      //   ),
      //   takeUntil(this._destroying$)
      // )
      .subscribe(() => {
        this.setLoginDisplay();
      });

    this.appSettingsApiService
      .getSettings()
      .subscribe(
        (appsettings) =>
          (this.festivalName =
            appsettings.festivalName ?? environment.festivalName)
      );
  }
  login() {
    this.authService.loginRedirect();
  }
  logout() {
    this.authService.logoutRedirect({
      postLogoutRedirectUri: environment.redirectUrl,
    });
  }
  setLoginDisplay() {
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
  }
}
