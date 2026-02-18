import { Component, OnInit, inject } from '@angular/core';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: 'confirmation-dialog.component.html',
  styleUrls: ['confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent implements OnInit {
  private activeModal = inject(NgbActiveModal);

  title: string = '';
  message: string = '';
  btnOkText: string = 'OK';
  btnCancelText: string = 'Cancel';

  ngOnInit() {}

  public decline() {
    this.activeModal.close(false);
  }

  public accept() {
    this.activeModal.close(true);
  }

  public dismiss() {
    this.activeModal.dismiss();
  }
}
