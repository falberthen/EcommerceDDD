import { Component, Input, OnInit, inject } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-confirmation-dialog',
    templateUrl: 'confirmation-dialog.component.html',
    styleUrls: ['confirmation-dialog.component.scss'],
    standalone: false
})
export class ConfirmationDialogComponent implements OnInit {
  private activeModal = inject(NgbActiveModal);

  @Input() title!: string;
  @Input() message!: string;
  @Input() btnOkText!: string;
  @Input() btnCancelText!: string;

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
