import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../Core/Models/User.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent  implements OnInit{

  public isTrustedUser :boolean = false;
  constructor(private builder: FormBuilder,private router : Router) {
    
    
  }
 loginForm = this.builder.group({
  userName : this.builder.control('user Name',Validators.compose([Validators.required])),
  password : this.builder.control('password',Validators.compose([Validators.required,Validators.minLength(8)]))
 })

 ngOnInit(): void {
  // this.loginForm.events.subscribe((event)=>{

  //   console.log(event);
  //   console.log(this.loginForm);
  // })
}

login() {
    if(this.loginForm.valid)
    {
      this.isTrustedUser= true;
      console.log('route to new ')
      this.router.navigate(['vendor']);
      let currentUser: User= {
        id: '',
        userName: this.loginForm.value.userName as string,
        password: this.loginForm.value.password as string,
        roles: []
      }

    }
    else
    {
      console.log('route to pidec ')
      this.router.navigate(['pidec/']);
    }
  }

}
