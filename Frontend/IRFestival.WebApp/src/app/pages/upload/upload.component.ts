import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { PicturesApiService } from 'src/app/api-services/pictures-api.service';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';
import { MsalBroadcastService, MsalService } from '@azure/msal-angular';
import { InteractionStatus } from '@azure/msal-browser';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
})
export class UploadComponent implements OnInit, OnDestroy {
  private readonly destroy$ = new Subject();
  loginDisplay: Boolean = false;
  uploadForm: FormGroup;
  private fileToUpload: File;
  msalBroadcastService: MsalBroadcastService;
  authService: MsalService;
  constructor(
    private picturesApiService: PicturesApiService,
    msalBroadcastService: MsalBroadcastService,
    authService: MsalService
  ) {
    this.msalBroadcastService = msalBroadcastService;
    this.authService = authService;
  }

  ngOnInit(): void {
    this.uploadForm = new FormGroup({
      file: new FormControl(undefined, Validators.required),
    });
    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None)
      )
      .subscribe(() => {
        this.loginDisplay =
          this.authService.instance.getAllAccounts().length > 0;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
  }

  onFileChange(event: InputEvent): void {
    const target = event.target as HTMLInputElement;
    if (!target.files || !target.files.length) {
      this.fileToUpload = null;
    }

    this.fileToUpload = target.files[0];
  }

  onFormSubmit(): void {
    if (!this.uploadForm.valid) {
      return;
    }

    this.picturesApiService
      .upload(this.fileToUpload)
      .pipe(takeUntil(this.destroy$))
      .subscribe();
  }
}
