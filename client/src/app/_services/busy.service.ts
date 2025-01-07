import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyReqeustCount = 0;
  private spinnerSevice = inject(NgxSpinnerService);

  busy() {
    this.busyReqeustCount++;
    this.spinnerSevice.show(undefined, {
      type: 'square-jelly-box',
      bdColor: 'rgba(252, 251, 251,0.2)',
      color: 'rgba(15, 14, 14, 0.87)',
      fullScreen : true
    });
  }

  idle() {
    this.busyReqeustCount--;
    if (this.busyReqeustCount <= 0) {
      this.busyReqeustCount = 0;
      this.spinnerSevice.hide();
    }
  }
}
