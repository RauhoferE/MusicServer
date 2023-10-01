import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { guidPattern, passwordPattern } from 'src/app/constants/patterns';
import { RegisterModel } from 'src/app/models/user-models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { birthdateValidator } from 'src/validators/birthdate.validator';
import { ConfirmedValidator } from 'src/validators/confirm.validator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  public registerForm: FormGroup;

  public showForm: boolean = true;

  /**
   *
   */
  constructor(private authservice: AuthenticationService,
    private fb: FormBuilder,
    private message: NzMessageService) {
      this.registerForm = this.fb.group({
        email: ['', [Validators.required, Validators.email]],
        username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(255)]],
        birthdate: [null, [Validators.required, birthdateValidator()]],
        password: ['', [Validators.required, Validators.pattern(passwordPattern)]],
        passwordConfirm: ['', [Validators.required, ]],
        registrationCode: ['', [Validators.required, Validators.pattern(guidPattern)]]
      }, {
        validator: ConfirmedValidator('password', 'passwordConfirm')
      })

  }

  submit(): void{
    if (this.registerForm.invalid) {
      Object.values(this.registerForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
      this.message.error('Error when registering in! \nPlease correct your input.')
      return;
    }

    //return;

    this.authservice.register({
      birth: this.birthdate?.value,
      email: this.email?.value,
      password: this.password?.value,
      registrationCode: this.registrationCode?.value,
      userName: this.username?.value
    } as RegisterModel).subscribe({
      next: () => {
        this.showForm = false;
      },
      error: (error) => {
        this.message.error(error.error.message);
      }
    })


  }

  get email(){
    return this.registerForm.get('email');
  }

  get password(){
    return this.registerForm.get('password');
  }

  get birthdate(){
    return this.registerForm.get('birthdate');
  }

  get username(){
    return this.registerForm.get('username');
  }

  get passwordConfirm(){
    return this.registerForm.get('passwordConfirm');
  }

  get registrationCode(){
    return this.registerForm.get('registrationCode');
  }

}
