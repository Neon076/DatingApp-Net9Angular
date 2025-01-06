import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dailog',
  standalone:true,
  imports: [],
  templateUrl: './confirm-dailog.component.html',
  styleUrl: './confirm-dailog.component.css'
})
export class ConfirmDailogComponent {
  bsModalRef = inject(BsModalRef);
  title = '';
  message = '';
  btnOkText= '';
  btnCancelText = '';
  result = false;

  confirm(){
    this.result = true;
    this.bsModalRef.hide();
  }

  decline(){
    this.bsModalRef.hide();
  }
}
