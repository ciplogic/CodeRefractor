/**
 * Created with IntelliJ IDEA.
 * User: BasicLaptop
 * Date: 9/23/13
 * Time: 8:21 PM
 * To change this template use File | Settings | File Templates.
 */
public class NBodySystem {
    private Body[] bodies;
    private Pair[] pairs;

    static final double Pi = 3.141592653589793;
    static final double Solarmass = 4 * Pi * Pi;
    static final double DaysPeryear = 365.24;

    public NBodySystem() {
        // Sun
        Body sun = new Body();
        sun.mass = Solarmass;

        Body jupiter = new Body();
        jupiter.x = 4.84143144246472090e+00;
        jupiter.y = -1.16032004402742839e+00;
        jupiter.z = -1.03622044471123109e-01;
        jupiter.vx = 1.66007664274403694e-03 * DaysPeryear;
        jupiter.vy = 7.69901118419740425e-03 * DaysPeryear;
        jupiter.vz = -6.90460016972063023e-05 * DaysPeryear;
        jupiter.mass = 9.54791938424326609e-04 * Solarmass;


        Body saturn = new Body(); // Saturn
        saturn.x = 8.34336671824457987e+00;
        saturn.y = 4.12479856412430479e+00;
        saturn.z = -4.03523417114321381e-01;
        saturn.vx = -2.76742510726862411e-03 * DaysPeryear;
        saturn.vy = 4.99852801234917238e-03 * DaysPeryear;
        saturn.vz = 2.30417297573763929e-05 * DaysPeryear;
        saturn.mass = 2.85885980666130812e-04 * Solarmass;

        Body uranus = new Body(); // Uranus
        uranus.x = 1.28943695621391310e+01;
        uranus.y = -1.51111514016986312e+01;
        uranus.z = -2.23307578892655734e-01;
        uranus.vx = 2.96460137564761618e-03 * DaysPeryear;
        uranus.vy = 2.37847173959480950e-03 * DaysPeryear;
        uranus.vz = -2.96589568540237556e-05 * DaysPeryear;
        uranus.mass = 4.36624404335156298e-05 * Solarmass;

        Body neptune= new Body(); // Neptune

        neptune.x = 1.53796971148509165e+01;
        neptune.y = -2.59193146099879641e+01;
        neptune.z = 1.79258772950371181e-01;
        neptune.vx = 2.68067772490389322e-03 * DaysPeryear;
        neptune.vy = 1.62824170038242295e-03 * DaysPeryear;
        neptune.vz = -9.51592254519715870e-05 * DaysPeryear;
        neptune.mass = 5.15138902046611451e-05 * Solarmass;

        bodies = new Body[5];
        bodies[0]=sun;
        bodies[1]=jupiter;
        bodies[2]=saturn;
        bodies[3]=uranus;
        bodies[4]=neptune;

        pairs = new Pair[bodies.length * (bodies.length-1)/2];
        int pi = 0;
        for (int i = 0; i < bodies.length-1; i++)
            for (int j = i+1; j < bodies.length; j++) {
                Pair pair = new Pair();
                pair.bi =bodies[i];
                pair.bj = bodies[j];

                pairs[pi++] = pair;
            }
        double px = 0.0, py = 0.0, pz = 0.0;
        for (Body b :bodies) {
            px += b.vx * b.mass; py += b.vy * b.mass; pz += b.vz * b.mass;
        }
        Body sol = bodies[0];
        sol.vx = -px/Solarmass; sol.vy = -py/Solarmass; sol.vz = -pz/Solarmass;
    }

    public void Advance(double dt) {
        for (Pair p : pairs) {
            Body bi = p.bi, bj = p.bj;
            double dx = bi.x - bj.x, dy = bi.y - bj.y, dz = bi.z - bj.z;
            double d2 = dx * dx + dy * dy + dz * dz;
            double mag = dt / (d2 * Math.sqrt(d2));
            bi.vx -= dx * bj.mass * mag; bj.vx += dx * bi.mass * mag;
            bi.vy -= dy * bj.mass * mag; bj.vy += dy * bi.mass * mag;
            bi.vz -= dz * bj.mass * mag; bj.vz += dz * bi.mass * mag;
        }
        for (Body b : bodies) {
            b.x += dt * b.vx; b.y += dt * b.vy; b.z += dt * b.vz;
        }
    }

    public double Energy() {
        double e = 0.0;
        for (int i = 0; i < bodies.length; i++) {
            Body bi = bodies[i];
            e += 0.5 * bi.mass * (bi.vx*bi.vx + bi.vy*bi.vy + bi.vz*bi.vz);
            for (int j = i+1; j < bodies.length; j++) {
                Body bj = bodies[j];
                double dx = bi.x - bj.x, dy = bi.y - bj.y, dz = bi.z - bj.z;
                e -= (bi.mass * bj.mass) / Math.sqrt(dx * dx + dy * dy + dz * dz);
            }
        }
        return e;
    }
}
