import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { passwordPattern } from 'src/app/constants/patterns';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ConfirmedValidator } from 'src/validators/confirm.validator';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent {

  public passwordForm: FormGroup;

  private userId: number = -1;

  private token: string = '';

  public submitted: boolean = false;
  /**
   *
   */
  constructor(private fb: FormBuilder,
    private authService: AuthenticationService,
    private message: NzMessageService,
    private route: ActivatedRoute) {

      this.passwordForm = this.fb.group({
        password: ['',[Validators.required, Validators.pattern(passwordPattern)]],
        passwordConfirm: ['',[Validators.required]]
      },{
        validator: ConfirmedValidator('password', 'passwordConfirm')
      })

      if (!this.route.snapshot.paramMap.has('userId') || !this.route.snapshot.paramMap.has('token')) {
        return;
      }

      const id = this.route.snapshot.paramMap.get('userId');

      if (isNaN(id as any)) {
        return;
      }

      this.userId = this.route.snapshot.paramMap.get('userId') as any;
      this.token = this.route.snapshot.paramMap.get('token') as any;
    
  }

  submit(): void{
    if (this.passwordForm.invalid) {
      Object.values(this.passwordForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error('Error when changing password! \nPlease correct your input.')
      return;
    }

    this.authService.resetPassword(this.userId, this.token, this.password?.value).subscribe({
      next: () => {
        this.submitted = true;
      },
      error: (error) => {
        this.message.error(error.error.message);
      }
    })


  }

  get password(){
    return this.passwordForm.get('password');
  }

  get passwordConfirm(){
    return this.passwordForm.get('passwordConfirm');
  }
}
