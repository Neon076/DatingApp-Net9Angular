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
      bdColor: 'rgba(0, 0, 0, 0.8)',
      color: '#fff',
      fullScreen : false
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
