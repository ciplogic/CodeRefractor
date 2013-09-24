import java.io.Console;

/**
 * Created with IntelliJ IDEA.
 * User: BasicLaptop
 * Date: 9/23/13
 * Time: 8:18 PM
 * To change this template use File | Settings | File Templates.
 */
public class NBody {
    public static void main(String[] args) {
        int n = 5000000;
        NBodySystem bodies = new NBodySystem();
        long start = System.currentTimeMillis();
        System.out.println(bodies.Energy());
        for (int i = 0; i < n; i++) bodies.Advance(0.01);
        System.out.println(bodies.Energy());

        long end = System.currentTimeMillis();
        System.out.println("Time: "+(end-start));
    }
}
